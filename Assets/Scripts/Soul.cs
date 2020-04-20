using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class Soul : MonoBehaviour
{
    public delegate void OnSoulDeath(Soul soul);
    public event OnSoulDeath SoulDeath;

    [Header("Сетап")]
    [SerializeField] private Collider collider;
    [SerializeField] private SphereCollider _playerRadar;
    [SerializeField] private SphereCollider _soulRadar;
    [SerializeField] private AudioSource _audioSource;

    public AudioClip ignitionSound;
    public AudioClip addSoulsSound;

    private List<Soul> souls = new List<Soul>();

    public GameObject DefaultSprite;
    public GameObject DeathSprite;
    public NavMeshAgent navMeshAgent;
    public GameObject triggers;

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

    private int _energyAfterDie;

    private Transform fieldCenter;

    private void OnEnable()
    {
        fieldCenter = FindObjectOfType<FieldCenter>().transform;
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
        _energyAfterDie = gameParemeters.energyForCatchSoul;
        navMeshAgent.speed = _randomWalkingSpeed;

        if (GameEconomy.curentItem)
        {
            foreach (ChangingParameter changingParameter in GameEconomy.curentItem.changingParameters)
            {
                TryAddBonusToParameter(changingParameter);
            }
        }
    }

    private void TryAddBonusToParameter(ChangingParameter changingParameter)
    {
        switch (changingParameter.gameParameter)
        {
            case GameParameter.AccelerationDuration: if (changingParameter.useAsPercent) _accelerationDuration *= changingParameter.value; else _accelerationDuration += changingParameter.value; break;
            case GameParameter.PlayerDetectionDistance: if (changingParameter.useAsPercent) _playerRadar.radius *= changingParameter.value; else _playerRadar.radius += changingParameter.value; break;
            case GameParameter.SlowdownDuration: if (changingParameter.useAsPercent) _slowdownDuration *= changingParameter.value; else _slowdownDuration += changingParameter.value; break;
            case GameParameter.SoulDetectionDistance: if (changingParameter.useAsPercent) _soulRadar.radius *= changingParameter.value; else _soulRadar.radius += changingParameter.value; break;
            case GameParameter.SoulSpeed: if (changingParameter.useAsPercent) _maxSpeed *= changingParameter.value; else _maxSpeed += changingParameter.value; break;
            case GameParameter.SoulWalkingSpeed: if (changingParameter.useAsPercent) navMeshAgent.speed *= changingParameter.value; else navMeshAgent.speed += changingParameter.value; break;
        }
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
            StartCoroutine(WaitToHoleEffect(other));

            DeathSprite.GetComponent<SpriteRenderer>().sortingOrder = 0;
            DeathSprite.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            Death();
            _animator.SetTrigger("DownDeath"); 
        }
        
        if(other.tag == "Geizer" && !isDead)
        {
            Death();
            _animator.SetTrigger("UpDeath");
        }

        if (other.tag == "Bush")
        {
            ignoreOtherSouls = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "GameFieldBound")
        {
            if (!navMeshAgent.enabled) { StopCoroutine("ActivateTriggers"); StartCoroutine("ActivateTriggers"); }
            navMeshAgent.enabled = true;
            alarm = false;
            navMeshAgent.SetDestination(fieldCenter.position);
            StopCoroutine("ChangingSoulWalkingDirection");
            triggers.SetActive(false);
        }    
    }

    private IEnumerator ActivateTriggers()
    {
        yield return new WaitForSeconds(2f);
        triggers.SetActive(true);
    }

    private IEnumerator WaitToHoleEffect(Collider other)
    {
        other.GetComponentInParent<Hole>().PlayCatchedEffect();
        yield return new WaitForSeconds(1f);
    }

    private void PlayIgnitionSound()
    {
        _audioSource.PlayOneShot(ignitionSound);
    }

    private void PlayAddSoulsSound()
    {
        _audioSource.PlayOneShot(addSoulsSound);
    }

    private void Death()
    {
        StopCoroutine("ChangingSoulWalkingDirection");
        alarm = false;
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
        navMeshAgent.enabled = false;
        triggers.SetActive(false);
        SoulDeath?.Invoke(this);
        Destroy(gameObject, 2f);
    }

    public void DeathByAnimation()
    {
        Death();
    }

    public void AddEnergy()
    {
        GamePlay.instance.TryAddEnergy(_energyAfterDie);
        _audioSource.PlayOneShot(addSoulsSound);
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
        if (!alarm) return;

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
