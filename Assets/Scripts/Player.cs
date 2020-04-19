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

    void OnEnable()
    {
        _speedWithoutSouls = FindObjectOfType<GameSettings>().GetGameParemeters().playerSpeed;
        _speedWithSouls = FindObjectOfType<GameSettings>().GetGameParemeters().playerSpeedWhenTargetingSouls;
        _curentSpeed = _speedWithoutSouls;
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

    void FixedUpdate()
    {
        if (curentSoulsTargeting > 0)
            _curentSpeed = _speedWithSouls;
        else
            _curentSpeed = _speedWithoutSouls;

        rigidbody.velocity = Vector3.zero;
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
