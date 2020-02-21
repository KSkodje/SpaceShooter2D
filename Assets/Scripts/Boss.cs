using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private int _bossHealth = 200;
    [SerializeField]
    private int _currentHealth = -1;
    [SerializeField]
    private int _damagePerHit = 10;
    [SerializeField]
    private float _speed = 1.5f;
    private float _canFire;
    private bool _dying = false;
    private bool _charging = false;
    private bool _invulnerable = false;
    [SerializeField]
    private char _facing;
    [SerializeField]
    private bool _turning = false;
    [SerializeField]
    private AudioClip _explosionSound = null;
    private AudioSource _audioSource;
    private Player _player = null;
    private Animator _anim = null;
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private GameObject _wingLasers = null;
    private bool _wingLasersOn = false;
    [SerializeField]
    private GameObject _enemyLaserPrefab = null;
    private GameObject _enemyLaser;
    [SerializeField]
    private GameObject _chargeShotPrefab = null;
    private GameObject _chargeShot = null;
    [SerializeField]
    private GameObject _targetedShotPrefab = null;
    private GameObject _targetedShot = null;
    [SerializeField]
    private GameObject _shield = null;

    private Quaternion targetDirection;
    // debug
    public bool _debug = true;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        if (!_audioSource)
        {
            Debug.Log("AudioSource on Explosion is Null");
        }
        else
        {
            _audioSource.clip = _explosionSound;
            _audioSource.volume = 0.6f;
            _audioSource.pitch = 1.5f;
        }
        Shields(true);


        targetDirection = Quaternion.Euler(0, 0, 0);

        if (_debug)
        {
            transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            transform.position = new Vector3(0, 9, 0);
        }
        
    }

    void Update()
    {
        CalculateMovement();

        if (transform.rotation != targetDirection)
        {
            _turning = true;
            if (_currentHealth < 100)
            {
                Shields(true);
                if(Random.Range(0,100) > 50)
                {
                    WingLasersToggle();
                }
            }
        }
        else
        {
            _turning = false;
        }


        if (_speed == 0 && Time.time > _canFire)
        {
            FiringLogic();
        }
    }

    IEnumerator Rotator()
    {
        while (!_dying)
        {
            /*
             * 0 - Stay
             * 1 - Center
             * 2 - Left (for the boss)
             * 3 - Right (for the boss)
             */
            int pickDir = Random.Range(0, 4);
            if (!_charging && !_turning)
            {
                switch (pickDir)
                {
                    case 0:
                        break;
                    case 1:
                        targetDirection = Quaternion.Euler(0, 0, 0);
                        break;
                    case 2:
                        targetDirection = Quaternion.Euler(0, 0, 90);
                        break;
                    case 3:
                        targetDirection = Quaternion.Euler(0, 0, -90);
                        break;
                    default:
                        Debug.LogError("Unexpected input: " + pickDir);
                        break;
                }
            }

            yield return new WaitForSeconds(5);
        }
        
    }

    void FiringLogic()
    {
        if (!_dying)
        {
            _canFire = Time.time + Random.Range(1f, 3f);
            float pickAttack = Random.Range(0f, 100f);
            //Debug.Log(pickAttack);
            float laserTypeThreshold = 50f;
            if (_currentHealth < 100)
            {
                laserTypeThreshold = 30f;
            }

            if (pickAttack < laserTypeThreshold)
            {
                FireLaser();
            }
            else if (laserTypeThreshold < pickAttack && pickAttack < 90f)
            {
                TargetedShot();
            }
            else if (pickAttack > 90f && !_turning)
            {
                if (_currentHealth < 100)
                {
                    StartCoroutine(ChargeShotBarrage());
                }
                else
                {
                    ChargeShot();
                }
            }
            else
            {
                TargetedShot();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (transform.position.y < 2.5f)
        {
            _speed = 0.75f;
        }
        if (transform.position.y <= 0)
        {
            _speed = 0f;
            Shields(false);
            _facing = 'c';
            if (_currentHealth == -1)
            {
                _currentHealth = _bossHealth;
                _uiManager.DisplayBossHealth(true);
                _uiManager.UpdateBossHealth(_currentHealth, _bossHealth);
                _canFire = Time.time + 1f;
                StartCoroutine(Rotator());
            }
        }
        switch (transform.rotation.eulerAngles.z)
        {
            case 90:
                _facing = 'l';
                break;
            case 270:
                _facing = 'r';
                break;
            case 0:
                _facing = 'c';
                break;
            default:
                _facing = 'x';
                break;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDirection, Time.deltaTime * 20f);
    }

    void FireLaser()
    {
        _enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, transform.rotation);
        Laser[] lasers = _enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
           
            lasers[i].AssignEnemyLasers();
        }
    }

    void ChargeShot()
    {
        _chargeShot = Instantiate(_chargeShotPrefab, transform.position, transform.rotation);
    }

    IEnumerator ChargeShotBarrage()
    {
        if (!_turning)
        {
            _charging = true;
            ChargeShot();
            yield return new WaitForSeconds(1f);
            ChargeShot();
            yield return new WaitForSeconds(.75f);
            ChargeShot();
            yield return new WaitForSeconds(.5f);
            ChargeShot();
            yield return new WaitForSeconds(.25f);
            _charging = false;
        }
        StopCoroutine(ChargeShotBarrage());
    }

    public void WingLasersToggle()
    {
        if (_wingLasersOn)
        {
            _wingLasersOn = false;
        }
        else
        {
            _wingLasersOn = true;
        }

        _wingLasers.SetActive(_wingLasersOn);
    }

    void TargetedShot()
    {
        _targetedShot = Instantiate(_targetedShotPrefab, transform.position, Quaternion.identity);
    }

    void OldRotate(char dir)
    {
        float rotationAngle = transform.rotation.eulerAngles.z;
        float rotationSpeed = 20f;

        if (dir == 'l')
        {
            // Boss rotates to its Left
            if (rotationAngle < 90 || rotationAngle > 268.5f)
            {
                transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
                Shields(true);
            }
            else
            {
                _facing = 'l';
                Shields(false);
            }

        }
        if (dir == 'r')
        {
            // Boss rotates to its right
            if (rotationAngle < 91.5f || rotationAngle > 270)
            {
                transform.Rotate(Vector3.back * Time.deltaTime * rotationSpeed);
                Shields(true);
            }
            else
            {
                _facing = 'r';
                Shields(false);
            }
        }
        if (dir == 'c')
        {
            // Boss rotates to center position
            if (rotationAngle > 0.1f && rotationAngle < 95)
            {
                transform.Rotate(Vector3.back * Time.deltaTime * rotationSpeed);
                Shields(true);
            }
            if (rotationAngle < 359.9f && rotationAngle > 265)
            {
                transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
                Shields(true);
            }
            if (rotationAngle < 0.1f || rotationAngle > 359.9f)
            {
                _facing = 'c';
                Shields(false);
            }
        }
        
    }

    void Shields(bool shieldsOn = false)
    {
        if (shieldsOn)
        {
            _invulnerable = true;
            _shield.SetActive(true);
        }
        else
        {
            _shield.SetActive(false);
            _invulnerable = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string hitBy = collision.tag;
        if (hitBy == "Laser" || hitBy == "Missile")
        {
            Destroy(collision.gameObject);
            TakeDamage();
        }
        if (hitBy == "Player")
        {
            Player player = collision.transform.GetComponent<Player>();
            if (player)
            {
                player.Damage();
            }
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (!_invulnerable)
        {
            _currentHealth -= _damagePerHit;
            _uiManager.UpdateBossHealth(_currentHealth, _bossHealth);
            if (_currentHealth <= 0)
            {
                OnBossDeath();
            }
        }
    }

    public void OnBossDeath()
    {
        _dying = true; // prevents it from being able to fire when death animation is triggered
        Shields(false);
        _player.AddToScore(1000);
        _uiManager.DisplayBossHealth(false);
        _audioSource.Play();
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        Destroy(this.gameObject, 2.37f);
        _uiManager.GameOverSequence(); //revamp later?
    }
}
