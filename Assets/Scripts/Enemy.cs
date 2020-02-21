using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.0f;

    private Player _player = null;
    private Vector3 _playerPos;
    private Animator _anim = null;

    [SerializeField]
    private GameObject _enemyFirePrefab = null;
    [SerializeField]
    private GameObject _enemyShieldVisualizer = null;
    [SerializeField]
    private bool _hasShield = false;
    [SerializeField]
    private float _fireRate = 3.0f;
    private float _canFire = 1.0f;
    private bool _dying = false;
    [SerializeField]
    private bool _smarterEnemy = false;
    [SerializeField]
    private float _ramDistance = 2.5f;
    [SerializeField]
    private bool _canDodge = false;
    [SerializeField]
    private bool _isAgressive = false;
    [SerializeField]
    private float _dodgeDistance = 1.7f;
    [SerializeField]
    private bool _isSideways = false;
    private char _dir;

    GameObject enemyLaser = null;
    [SerializeField]
    private AudioClip _explosionSound = null;
    private AudioSource _audioSource;

    private bool _pickupInRange = false;
    private float _hasTargetFireRate = 1.0f;
    private bool _fireAtPickups = false;

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
        // chose movement type if(movementtype == x) AlternateMovment, CalculateMovement etc
        // also set movement direction right or left

        //_speed = 0;
        EnemyShield();
    }

    void Update()
    {
        if (_isSideways)
        {
            AlternateMovement();
        }
        else
        {
            CalculateMovement();
        }

        ShootPowerups();
        // if Alternate movement, direction has to be set based on enemy rotation and laserfire needs to be adjusted as well
        if (Time.time > _canFire && !_dying && transform.position.y < 6)
        {
            FireLaser();
        }
        if (Time.time > _hasTargetFireRate && !_dying && transform.position.y < 6 && _pickupInRange)
        {
            FireLaser();
        }
    }

    //public void SetAbilities(float enemySpeed, float enemyFireRate, bool canFireAtPickups, bool canSpawnFromSide, bool canHaveShield, bool canBeAgressive, bool canFireBackwards, bool canEvade)
    public void SetAbilities(float enemySpeed, float enemyFireRate, bool canFireAtPickups, char spawnDir, bool canHaveShield, bool canBeAgressive, bool canFireBackwards, bool canEvade)
    {
        _speed = enemySpeed;
        _fireRate = enemyFireRate;
        if (canFireAtPickups)
        {
            _fireAtPickups = FlipCoin();
        }
        if (canHaveShield)
        {
            _hasShield = FlipCoin();
        }
        if (canBeAgressive)
        {
            _isAgressive = FlipCoin();
        }
        if (canFireBackwards)
        {
            _smarterEnemy = FlipCoin();
        }
        if (canEvade)
        {
            _canDodge = FlipCoin();
        }
        if (spawnDir != 't')
        {
            _canDodge = false;
            _isAgressive = false;
            _isSideways = true;
            _dir = spawnDir;
        }
    }

    private bool FlipCoin()
    {
        int flip = Random.Range(0, 10);
        if (flip < 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void FireLaser()
    {
        if (_pickupInRange)
        {
            _fireRate = 1f;
            _hasTargetFireRate = Time.time + _fireRate;
        }
        else
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
        }
        Laser();
    }

    void Laser()
    {
        if (_isSideways)
        {
            if (_dir == 'l')
            {
                enemyLaser = Instantiate(_enemyFirePrefab, transform.position, Quaternion.Euler(0, 0, -90));
            }
            else if (_dir == 'r')
            {
                enemyLaser = Instantiate(_enemyFirePrefab, transform.position, Quaternion.Euler(0, 0, 90));
            }
        }
        else
        {
            enemyLaser = Instantiate(_enemyFirePrefab, transform.position, Quaternion.identity);
        }
        //GameObject enemyLaser = Instantiate(_enemyFirePrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            if (_smarterEnemy && !_isSideways && _player.transform.position.y > transform.position.y)
            {
                lasers[i].EnemyBackwardsFire();
            }
            else if (_isSideways)
            {
                // checks if direction and position of player relative to enemy back - if 'smarter enemy' fire backwards
                if (_player.transform.position.x < transform.position.x && _dir == 'r' && _smarterEnemy)
                {
                    lasers[i].SidewaysAndBackwardsFire(_dir);
                }
                else if (_player.transform.position.x > transform.position.x && _dir == 'l' && _smarterEnemy)
                {
                    lasers[i].SidewaysAndBackwardsFire(_dir);
                }
                else
                {
                    lasers[i].SidewaysEnemy(_dir);
                }
            }
            else
            {
                lasers[i].AssignEnemyLasers();
            }
        }
    }


    void CalculateMovement()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (transform.position.y < -7.5f)
        {
            float randXpos = Random.Range(-9f, 9f);
            transform.position = new Vector3(randXpos, 7, 0);
        }
    }
    
    void AlternateMovement()
    {
        if(_dir == 'l')
        {
            transform.rotation = Quaternion.Euler(0, 0, 0 - 90);
            transform.Translate(Vector3.down * Time.deltaTime * _speed);
            if( transform.position.x < -11.5f)
            {
                float randYpos = Random.Range(-3f, 6f);
                transform.position = new Vector3(12f, randYpos, 0);
            }
        }
        if (_dir == 'r')
        {
            transform.rotation = Quaternion.Euler(0, 0, 0 + 90);
            transform.Translate(Vector3.down * Time.deltaTime * _speed);
            if (transform.position.x > 11.5f)
            {
                float randYpos = Random.Range(-3f, 6f);
                transform.position = new Vector3(-12f, randYpos, 0);
            }
        }
    }

    void EvasiveManeuver()
    {
        GameObject[] playerLasers = GameObject.FindGameObjectsWithTag("Laser");
        if (playerLasers.Length != 0)
        {
            foreach(GameObject laser in playerLasers)
            {
                Vector3 laserPos = laser.transform.position;
                if(Vector3.Distance(transform.position, laserPos) < _dodgeDistance)
                {
                    if(laserPos.x > transform.position.x)
                    {
                        transform.Translate(Vector3.left * Time.deltaTime * 1f);
                    }
                    else
                    {
                        transform.Translate(Vector3.right * Time.deltaTime * 1f);
                    }
                }
            }
        }
    }

    void AggressiveBehaviour()
    {
        // moves towards the player if within _ramDistance
        Vector3 _playerPos = _player.transform.position;
        float dist = Vector3.Distance(_playerPos, transform.position);
     
        if (dist <= _ramDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerPos, Time.deltaTime * 1f);
        }
        
    }

    void EnemyShield()
    {
        if (_hasShield)
        {
            _enemyShieldVisualizer.SetActive(true);
        }
        else
        {
            _enemyShieldVisualizer.SetActive(false);
        }
    }

    void ShootPowerups()
    {
        Powerup[] powerups = GameObject.FindObjectsOfType<Powerup>();
        if (powerups.Length == 0)
        {
            _pickupInRange = false;
        }
        else
        {
            GameObject target = FindClosest(powerups);
            if (target)
            {
                _pickupInRange = true;
            }
            else
            {
                _pickupInRange = false;
            }
        }
    }

    // Will have to alter or rework this function for the alternate movement type enemy
    private GameObject FindClosest(Powerup[] powerups)
    {
        GameObject chosenTarget = null;
        foreach (Powerup p in powerups)
        {
            // if powerup is below enemy and within certain distance from enemy center horizontally - fire
            if (-0.7f < (p.transform.position.x - transform.position.x) && (p.transform.position.x - transform.position.x) < 0.7f && (p.transform.position.y < (transform.position.y - 3f)))
            {
                //_pickupInRange = true;
                chosenTarget = p.gameObject;
                break;
            }
        }
        // or have a list and ForEach target in range shoot and then push to another list, if 
        return chosenTarget;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        string hitBy = other.tag;
        //Debug.Log("Hit by: " + hitBy);
        
        if (hitBy == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player)
            {
                player.Damage();
            }
            OnEnemyDeath();
        }

        if (hitBy == "Laser" || hitBy == "Missile")
        {
            Destroy(other.gameObject);
            if (!_hasShield)
            {
                Destroy(this.GetComponent<BoxCollider2D>()); // to enable the laser to pass through the explosion. Just deactivating isTrigger did not seem to do it.
                if (_player)
                {
                    _player.AddToScore(10);
                }
            }
            OnEnemyDeath();
        }

        if(hitBy == "Enemy_Laser") // Friendly fire is off
        {
            Destroy(other.gameObject);
        }
    }

    public void OnEnemyDeath()
    {
        if(_hasShield == true)
        {
            _hasShield = false;
            _enemyShieldVisualizer.SetActive(false);
        }
        else
        {
            _dying = true; // prevents it from being able to fire if death animation is triggered
            _audioSource.Play();
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(this.gameObject, 2.37f);
        }
    }
}
