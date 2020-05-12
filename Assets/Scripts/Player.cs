using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public delegate void OnPlayerDie();
    public static event OnPlayerDie PlayerDie;

    public delegate void OnPlayerDamaged(int curentLifes);
    public static event OnPlayerDamaged PlayerDamaged;

    public Rigidbody rigidbody;
    public Animator animator;
    public Transform playerCenter;

    [HideInInspector] public float _speedWithoutSouls;
    [HideInInspector] public float _speedWithSouls;
    public float stepSoundPeriod;
    public AudioSource audioSource;
    public AudioClip[] normalStepClips;
    public AudioClip[] iceStepClips;
    public AudioClip holyWall;

    [SerializeField] private float _curentSpeed;
    private Joystick joystick;

    [HideInInspector] public int curentSoulsTargeting;

    private bool needPlayStep;
    public bool onIce;
    public float maxVelocity;
    public float speedUpDuration;
    public float playerPushFromHolyWallForce;

    [SerializeField] private int _playerLifesCount = 1;
    private bool _isShoked;
    private bool _Died;

    private bool increasingSpeed;

    private Tween curentTween;

    void OnEnable()
    {
        AddListeners();
        joystick = FindObjectOfType<Joystick>();
        StartCoroutine("StepSounds");

        if (!GameEconomy.curentItem) return;
    }

    private void AddListeners()
    {
        GameSettings.GameSettingsLoaded += GameSettingsLoaded;
    }

    private void GameSettingsLoaded(GameParemeters gameParemeters)
    {
        _speedWithoutSouls = gameParemeters.playerSpeed;
        _speedWithSouls = gameParemeters.playerSpeedWhenTargetingSouls;
        _curentSpeed = _speedWithoutSouls;
        _playerLifesCount = gameParemeters.playerLifes;
        speedUpDuration = gameParemeters.speedUpDuration;
        maxVelocity = gameParemeters.maxIceVelocitySpeed;
        playerPushFromHolyWallForce = gameParemeters.pushFromHolyWallForce;

        _speedWithoutSouls = GameEconomy.curentItem.TryModifyParameter(GameParameter.PlayerSpeed, _speedWithoutSouls);
        _playerLifesCount = (int)GameEconomy.curentItem.TryModifyParameter(GameParameter.PlayerLifesCount, _playerLifesCount);
    }

    private void OnDisable()
    {
        StopCoroutine("StepSounds");
    }

    private IEnumerator StepSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepSoundPeriod);
            if (needPlayStep)
            {
                if(onIce)
                    audioSource.PlayOneShot(iceStepClips[Random.Range(0, iceStepClips.Length)]);
                else
                    audioSource.PlayOneShot(normalStepClips[Random.Range(0, normalStepClips.Length)]);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Holywall")
        {
            _playerLifesCount--;

            if (_playerLifesCount == 0)
            {
                _Died = true;
                animator.SetTrigger("PlayerDamaged");
                PlayerDie();
            }
            else
            {
                audioSource.PlayOneShot(holyWall);
                PlayerDamaged?.Invoke(_playerLifesCount);
                _isShoked = true;
                animator.SetTrigger("PlayerDamaged");

                Vector3 playerPushDirection = playerCenter.position - collision.contacts[0].point;
                playerPushDirection.y = 0f;

                rigidbody.AddForce((playerPushDirection).normalized * playerPushFromHolyWallForce, ForceMode.Impulse);

                float returnToNormal = 0f;
                DOTween.To(() => returnToNormal, x => returnToNormal = x, 1, 1f).OnComplete(()=> { _isShoked = false; });
            }
        }
    }

    [ContextMenu("Test")]
    public void Test()
    {
        rigidbody.AddForce((Vector3.zero - transform.position).normalized * playerPushFromHolyWallForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ice")
            onIce = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ice")
            onIce = false;
    }

    void FixedUpdate()
    {
        if (_isShoked) return;
        if (_Died) return;

        if (Input.GetKeyDown(KeyCode.Space)) onIce = !onIce;

        if (onIce)
        {
            _curentSpeed = _speedWithoutSouls / 10f;
        }

        if (!onIce)
        {
            rigidbody.velocity = Vector3.zero;
        }

        else if (rigidbody.velocity.magnitude > maxVelocity)
        {
            _curentSpeed = 0;
        }

        rigidbody.AddForce(transform.forward * _curentSpeed * joystick.Vertical);
        rigidbody.AddForce(transform.right * _curentSpeed  * joystick.Horizontal);

        if (joystick.Vertical != 0f || joystick.Horizontal != 0f)
        {
            needPlayStep = true;
            animator.SetBool("Moving", true);
        }
        else
        {
            needPlayStep = false;
            animator.SetBool("Moving", false);
        }

        if (onIce) return;

        if(joystick.Vertical == 0f && joystick.Horizontal == 0f)
        {
             curentTween.Kill();
            _curentSpeed = 0f;
            curentTween = null;

            increasingSpeed = false;
        } else
        {
            if (!increasingSpeed)
            {
                increasingSpeed = true;

                curentTween = DOTween.To(() => _curentSpeed, x => _curentSpeed = x, _speedWithoutSouls, speedUpDuration);
            }
        }
    }
}
