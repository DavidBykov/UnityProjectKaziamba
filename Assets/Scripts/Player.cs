using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;

    [HideInInspector] public float _speedWithoutSouls;
    [HideInInspector] public float _speedWithSouls;
    public float stepSoundPeriod;
    public AudioSource audioSource;
    public AudioClip[] clips;

    private float _curentSpeed;
    private Joystick joystick;

    [HideInInspector] public int curentSoulsTargeting;

    private bool needPlayStep;
    public bool onIce;
    public float maxVelocity;

    void OnEnable()
    {
        AddListeners();
        joystick = FindObjectOfType<Joystick>();

        if (GameEconomy.curentItem)
        {
            foreach (ChangingParameter changingParameter in GameEconomy.curentItem.changingParameters)
            {
                if (changingParameter.gameParameter == GameParameter.PlayerSpeed)
                {
                    if (changingParameter.useAsPercent)
                        _speedWithoutSouls *= (int)changingParameter.value;
                    else
                        _speedWithoutSouls += (int)changingParameter.value;
                }
            }
            _curentSpeed = _speedWithoutSouls;
        }

        StartCoroutine("Step");
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
    }

    private void OnDisable()
    {
        StopCoroutine("Step");
    }

    private IEnumerator Step()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepSoundPeriod);
            if (needPlayStep) audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ice")
            onIce = true;

        if(other.tag == "Holywall")
        {
            FindObjectOfType<GamePlay>().playerKilled = true;
            FindObjectOfType<GamePlay>().CheckGameEndedCondition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ice")
            onIce = false;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) onIce = !onIce;

        if (onIce)
        {
            _curentSpeed = _speedWithoutSouls / 10f;
        } else
        {
            _curentSpeed = _speedWithoutSouls;
        }

        //if (curentSoulsTargeting > 0)
        //_curentSpeed = _speedWithSouls;
        //else
        //_curentSpeed = _speedWithoutSouls;

        if (!onIce)
        {
            rigidbody.velocity = Vector3.zero;
        }
        else if (rigidbody.velocity.magnitude > maxVelocity)
        {
            _curentSpeed = 0;
        }
        rigidbody.AddForce(transform.forward * _curentSpeed * joystick.Vertical);
        //rigidbody.AddForce(transform.forward * _speed * -1f);
        rigidbody.AddForce(transform.right * _curentSpeed * joystick.Horizontal);


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
    }
}
