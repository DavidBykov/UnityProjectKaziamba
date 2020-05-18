using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class Soul : MonoBehaviour
{
    public delegate void OnSoulDeath(Soul soul, bool addEnergy);
    public event OnSoulDeath SoulDeath;

    public delegate void OnNeedAddEnergy(float energyAfterDie);
    public event OnNeedAddEnergy NeedAddEnergy;

    [Header("Сетап")]
    [SerializeField] private Collider collider;
    [SerializeField] private SphereCollider _playerRadar;
    [SerializeField] private SphereCollider _soulRadar;
    [SerializeField] private AudioSource _audioSource;

    public AudioClip ignitionSound;
    public AudioClip addSoulsSound;

    private List<Soul> souls = new List<Soul>();

    public GameObject DefaultSprite;
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
    public bool ignoreOtherSouls;

    private Vector3 finalDestination;
    private Vector3 soulsVectorsSum;

    private Rigidbody _rigidbody;
    public Animator soulStateAnimator;
    private Animator _deathAnimator;

    private GameSettings _gameSettings;

    private float distanceToPlayer;
    private float curentDistanceToMaxDistanceRatio;
    private float curentWeight;

    private float _randomWalkingSpeed;
    private Vector3 _randomWalkingVector;
    private Vector2 _randomWalkingChangeDirectionPeriod;

    public bool debug;

    private int _energyAfterDie;

    private Transform fieldCenter;

    public float savedVelocity;
    public float finalPoint;
    public float soulFinalSpeed;
    public float distanceToBounds;
    public float runningFromBoundsSpeed;
    public Text debugText;

    private bool runningAwayBounds;
    private Vector3 boundsPosition;

    private bool prepareToCatch;
    private Transform deathHole;

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
        _deathAnimator = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        _gameSettings = FindObjectOfType<GameSettings>();
        ApplySettings(_gameSettings.GetGameParemeters());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Magnet" && prepareToCatch == false)
        {
            prepareToCatch = true;
            deathHole = other.transform;
        }

        if (other.tag == "Soul" && !isDead)
        {
            //Debug.Log(other.name);
            souls.Add(other.GetComponentInParent<Soul>());
        }

        if (other.tag == "Player" && !isDead)
        {
            soulStateAnimator.SetInteger("SoulState", 1);
            Debug.Log(other.name);
            savedVelocity = _rigidbody.velocity.magnitude;
            _player.curentSoulsTargeting++;
            StopCoroutine("SettingAlarmFalse");
            navMeshAgent.enabled = false;
            //DOTween.To(() => _currentSpeed, x => _currentSpeed = x, _maxSpeed, _accelerationDuration);

            StopCoroutine("ChangingSoulWalkingDirection");
            //_deathAnimator.SetBool("Scary", true);
            alarm = true;
        }

        if (other.tag == "Fire" && !isDead)
        {
            StartCoroutine(WaitToHoleEffect(other));

            DefaultSprite.GetComponent<SpriteRenderer>().sortingOrder = 0;
            DefaultSprite.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            Death(false, false);
            soulStateAnimator.SetInteger("SoulState", 2);
            _deathAnimator.SetTrigger("DownDeath"); 
        }
        
        if(other.tag == "Geizer" && !isDead)
        {
            Death(true, false);
            soulStateAnimator.SetInteger("SoulState", 2);
            _deathAnimator.SetTrigger("UpDeath");
        }

        if (other.tag == "Bush")
        {
            ignoreOtherSouls = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Soul" && !isDead)
        {
            souls.Remove(other.GetComponentInParent<Soul>());
        }

        if (other.tag == "Player" && !isDead)
        {
            savedVelocity = _rigidbody.velocity.magnitude;

            alarm = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        //return;
        //if (collision.collider.tag == "Player") Physics.IgnoreCollision(collider, collision.collider, true);
        if(collision.collider.tag == "GameFieldBound")
        {
            if (runningAwayBounds) return;
            if (!alarm) return; 
            ////if (!navMeshAgent.enabled) { StopCoroutine("ActivateTriggers"); StartCoroutine("ActivateTriggers"); }
            navMeshAgent.enabled = false;
            savedVelocity = _rigidbody.velocity.magnitude;
            triggers.SetActive(false);
            alarm = false;
            runningAwayBounds = true;
            boundsPosition = collision.contacts[0].point;

            //StopCoroutine("ChangingSoulWalkingDirection");
            //triggers.SetActive(false);
        }    
    }

    private IEnumerator ActivateTriggers()
    {
        yield return new WaitForSeconds(0.5f);
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

    private void Death(bool triggerDeathEvent, bool tryAddenergy)
    {
        StopCoroutine("ChangingSoulWalkingDirection");
        alarm = false;
        _audioSource.Play();

        _currentSpeed = 0f;
        isDead = true;
        collider.enabled = false;
        _rigidbody.isKinematic = true;
        _deathAnimator.SetBool("Scary", false);
        navMeshAgent.enabled = false;
        triggers.SetActive(false);
        if(triggerDeathEvent) SoulDeath?.Invoke(this, tryAddenergy);
        Destroy(gameObject, 2f);
    }

    public void DeathByAnimation()
    {   
        SoulDeath?.Invoke(this, true);
    }

    public void AddEnergy()
    {
        NeedAddEnergy?.Invoke(_energyAfterDie);
        _audioSource.PlayOneShot(addSoulsSound);
    }

    private IEnumerator SettingAlarmFalse()
    {
        yield return new WaitForSeconds(2f);
        soulStateAnimator.SetInteger("SoulState", 0);
        _deathAnimator.SetBool("Scary", false);
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
        //if (debug) Debug.Log(_rigidbody.velocity.magnitude);

        if (isDead) return;
        if (navMeshAgent.enabled) return;

        if (prepareToCatch)
        {
            finalDestination = Vector3.Normalize(deathHole.position - transform.position);

            _rigidbody.velocity = new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * 1.66f;
            return;
        }

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
        if (!runningAwayBounds)
            finalDestination = (transform.position - _player.transform.position) + soulsVectorsSum * curentWeight;
        else
            finalDestination = fieldCenter.position - transform.position;

        if(runningAwayBounds && Vector3.Distance(transform.position, boundsPosition) > distanceToBounds || (runningAwayBounds && Vector3.Distance(_player.transform.position, boundsPosition) < Vector3.Distance(transform.position, boundsPosition)))
        {
            savedVelocity = 0;
            StopCoroutine("ChangingSoulWalkingDirection");
            StartCoroutine("ChangingSoulWalkingDirection");
            alarm = false;
            runningAwayBounds = false;
            triggers.SetActive(true);
        }
        // Если игрок в радиусе души, то прикладываем силу и толкаем душу в вычисленном направлении
        if (alarm)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
            distanceToPlayer -= finalPoint;
            if (distanceToPlayer < 0) distanceToPlayer = 0;

            float coef = (_playerRadar.radius - finalPoint - distanceToPlayer) / (_playerRadar.radius - finalPoint);

            if (coef < 0) coef = 0;
            if (coef > 1) coef = 1;

            if (debugText) debugText.text = "Дистанция до игрока " + distanceToPlayer + "\n" + " Радиус радара" + _playerRadar.radius + "\n" + " Каеф " + coef + "\n" + " Скорость " + _rigidbody.velocity.magnitude;

            //if (debug) Debug.Log("Дистанция до игрока " + distanceToPlayer + " Радиус радара" + _playerRadar.radius + " Каеф " + coef);
            //_rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
            float resultSpeed = savedVelocity + (soulFinalSpeed - savedVelocity) * coef;
            if (resultSpeed < savedVelocity) resultSpeed = savedVelocity;

            _rigidbody.velocity = new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * resultSpeed;
            //_rigidbody.velocity = new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * (3.6f);

            //_rigidbody.AddForce(new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * _currentSpeed);
        } else if (!navMeshAgent.enabled)
        {
            if (runningAwayBounds)
            {
                _rigidbody.velocity = new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * (runningFromBoundsSpeed);
            } else
            {
                _rigidbody.velocity = new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * 0.66f;
            }
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
