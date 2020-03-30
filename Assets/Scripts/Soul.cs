using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class Soul : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private SphereCollider _playerRadar;
    [SerializeField] private SphereCollider _soulRadar;
    [SerializeField] private AudioSource _audioSource;

    private List<Soul> souls = new List<Soul>();

    public GameObject DefaultSprite;
    public GameObject DeathSprite;
    public NavMeshAgent navMeshAgent;

    public float vectorLenght;

    private float _maxSpeed;
    private float _currentSpeed;
    private float _accelerationDuration;
    private float _slowdownDuration;
    private AnimationCurve _weightCurve;

    private Player _player;
    private bool alarm;
    private bool isDead;
    private bool ignoreOtherSouls;

    private Vector3 finalDestination;
    private Vector3 soulsVectorsSum;

    private Rigidbody _rigidbody;
    private Animator _animator;

    private GameSettings _gameSettings;

    private float distanceToPlayer;
    private float curentDistanceToMaxDistanceRatio;
    private float curentWeight;

    private float _randomWalkingSpeed;
    private Vector3 _randomWalkingVector;
    private Vector2 _randomWalkingChangeDirectionPeriod;

    private void OnEnable()
    {
        AddListeners();
        GetComponents();

        StartCoroutine("ChangingSoulWalkingDirection");
    }

    private void OnDisable()
    {
        DeleteListeners();
    }

    private void AddListeners()
    {
        TestUI.TestUISettingsChanged += TestUISettingsChanged;
        GameSettings.GameSettingsLoaded += GameSettingsLoaded;
    }

    private void DeleteListeners()
    {
        GameSettings.GameSettingsLoaded -= GameSettingsLoaded;
    }

    private void TestUISettingsChanged()
    {
        //throw new System.NotImplementedException();
    }

    private void GameSettingsLoaded(GameParemeters gameParemeters)
    {
        ApplySettings(gameParemeters);
    }

    private void ApplySettings(GameParemeters gameParemeters)
    {
        _playerRadar.radius = gameParemeters.playerDetectionDistance;
        _soulRadar.radius = gameParemeters.soulDetectionDistance;
        _maxSpeed = gameParemeters.soulSpeed;
        _accelerationDuration = gameParemeters.accelerationDuration;
        _slowdownDuration = gameParemeters.slowdownDuration;
        _weightCurve = gameParemeters.behaviourWeightByDistanceCurve;
        _randomWalkingSpeed = gameParemeters.soulWalkingSpeed/100f;
        _randomWalkingChangeDirectionPeriod = gameParemeters.soulWalkingChangeDirectionPeriod;
        navMeshAgent.speed = _randomWalkingSpeed;
    }

    private void GetComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        _gameSettings = FindObjectOfType<GameSettings>();
        ApplySettings(_gameSettings.GetGameParemeters());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Soul" && !isDead)
        {
            //Debug.Log(other.name);
            souls.Add(other.GetComponentInParent<Soul>());
        }

        if (other.tag == "Player" && !isDead)
        {
            _player.curentSoulsTargeting++;
            StopCoroutine("SettingAlarmFalse");
            navMeshAgent.enabled = false;
            _currentSpeed = _maxSpeed;
            //DOTween.To(() => _currentSpeed, x => _currentSpeed = x, _maxSpeed, _accelerationDuration);

            StopCoroutine("ChangingSoulWalkingDirection");
            _animator.SetBool("Scary", true);
            alarm = true;
        }

        if (other.tag == "Fire" && !isDead)
        {
            GamePlay.instance.AddSouls();
            Death();
        }
        
        if(other.tag == "Geizer" && !isDead)
        {
            Death();
        }

        if (other.tag == "Bush")
        {
            ignoreOtherSouls = true;
        }
    }

    private void Death()
    {
        StopCoroutine("ChangingSoulWalkingDirection");
        _audioSource.Play();
        //if(alarm)
        //_player.curentSoulsTargeting--;

        _currentSpeed = 0f;
        DefaultSprite.SetActive(false);
        DeathSprite.SetActive(true);
        isDead = true;
        collider.enabled = false;
        _rigidbody.isKinematic = true;
        _animator.SetBool("Scary", false);
        _animator.SetTrigger("Death");
        navMeshAgent.enabled = false;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Soul" && !isDead)
        {
            souls.Remove(other.GetComponentInParent<Soul>());
        }

        if (other.tag == "Player" && !isDead)
        {
            StopCoroutine("ChangingSoulWalkingDirection");
            StartCoroutine("ChangingSoulWalkingDirection");
            StartCoroutine("SettingAlarmFalse");
            _player.curentSoulsTargeting--;
        }

        if (other.tag == "Bush")
        {
            ignoreOtherSouls = false;
        }
    }

    private IEnumerator SettingAlarmFalse()
    {
        yield return new WaitForSeconds(_slowdownDuration);
        _animator.SetBool("Scary", false);
        _currentSpeed = 0f;
        alarm = false;
    }
    
    private IEnumerator ChangingSoulWalkingDirection()
    {
        yield return new WaitForSeconds(1f);
        navMeshAgent.enabled = true;

        while (true)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 5f;

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 5f, 1);
            Vector3 finalPosition = hit.position;

            navMeshAgent.SetDestination(finalPosition);
            _randomWalkingVector = new Vector3(Random.Range(-360, 360), 0f, Random.Range(-360, 360));
            yield return new WaitForSeconds(Random.Range(_randomWalkingChangeDirectionPeriod.x, _randomWalkingChangeDirectionPeriod.y));
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        
        finalDestination = Vector3.Normalize(transform.position - _player.transform.position);
        soulsVectorsSum = Vector3.zero;
        
        // Складываем векторы всех душ в радиусе, тем самым высчитывая общее направление
        foreach (Soul soul in souls)
        {
            if (soul.alarm && !soul.isDead)
                soulsVectorsSum += (soul.transform.position - _player.transform.position);
        }

        // Высчитываем, насколько общее направление должно влиять на собственное направление души, исходя из кривой в настройках
        distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        curentDistanceToMaxDistanceRatio = distanceToPlayer / _playerRadar.radius;
        curentWeight = _weightCurve.Evaluate(curentDistanceToMaxDistanceRatio);
        if (ignoreOtherSouls) curentWeight = 0f;

        // Считаем результирующее направление c учетом общего направления и его веса
        finalDestination = (transform.position - _player.transform.position) + soulsVectorsSum * curentWeight;

        // Если игрок в радиусе души, то прикладываем силу и толкаем душу в вычисленном направлении
        if (alarm)
        {
            _rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
            _rigidbody.AddForce(new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * _currentSpeed);
        } else
        {
            //_rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
            //_rigidbody.AddForce(new Vector3(_randomWalkingVector.normalized.x, 0f, _randomWalkingVector.normalized.z) * _randomWalkingSpeed);
        }

        DrawDirectionInfo(finalDestination);
    }

    private void DrawDirectionInfo(Vector3 direction)
    {
        Color directionColor;
        if (alarm) directionColor = Color.red; else directionColor = Color.green;

        Debug.DrawRay(transform.position, Vector3.Normalize(transform.position - _player.transform.position) * vectorLenght, Color.yellow);
        Debug.DrawRay(transform.position, direction * vectorLenght, directionColor);
    }
}
