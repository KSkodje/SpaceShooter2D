using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // SerializeField here used to have the field visible in the Unity Editor Inspector (private variables otherwise does not show)
    [SerializeField]
    private float _setSpeed = 3.5f;
    [SerializeField]
    private float _actualSpeed = 0;
    private float _speedMultiplier = 2;
    private bool _thrusterActivated = false;

    [SerializeField]
    private float _fireRate = 0.2f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _ammoCount = 15;
    [SerializeField]
    private int _maxAmmo = 30;

    [SerializeField]
    private bool _hasTripleShot = false;
    private bool _hasSpeedBoost = false;
    private bool _hasShield = false;
    [SerializeField]
    private bool _hasHomingShot = false;

    // Prefabs and links
    [SerializeField]
    private GameObject _laserPrefab = null;
    [SerializeField]
    private GameObject _tripleShotPrefab = null;
    [SerializeField]
    private GameObject _homingShotPrefab = null;
    //[SerializeField]
    //private GameObject _playerShieldPrefab = null;
    [SerializeField]
    private SpawnManager _spawnManager = null;
    [SerializeField]
    private UIManager _uiManager = null;
    [SerializeField]
    private GameObject _shieldVisualizer = null;
    private int _shieldStrength = 0;
    [SerializeField]
    private GameObject[] _playerHurtVisualizer = null;
    [SerializeField]
    private GameObject _thruster = null;
    [SerializeField]
    private float _thrusterBoost = 3.0f;
    [SerializeField]
    private float _maxThrusterFuel = 5.0f;
    [SerializeField]
    private float _currentThrusterFuel = 5.0f;
    private bool thrusterDisabled = false;

    [SerializeField]
    private int _score = 0;
    private int _previousDamage = -1; // set to -1 - changes when taking damage to randomize which wing gets the damage

    [SerializeField]
    private AudioClip _laserShotSound = null;
    private AudioSource _audioSource = null;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (!_audioSource)
        {
            Debug.LogError("AudioSource on Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserShotSound;
        }
        if (!_uiManager)
        {
            Debug.Log("UIManager is Null");
        }
        //_spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();//nullcheck
        transform.position = new Vector3(0, -2, 0);
        _uiManager.UpdateAmmo(_ammoCount, _maxAmmo);
    }

    void Update()
    {        
        CalculateMovement();
        ThrusterUsage();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount >0)
        {
            FireLaser();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AmmoHandler(20);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Simulate 4th powerup
            _spawnManager.RandomEnemySelfDestruct();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            FixDamage();
        }
    }

    void ThrusterUsage()
    {
        // input handling
        if (Input.GetKeyDown(KeyCode.LeftShift) && !thrusterDisabled)
        {
            _thrusterActivated = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _thrusterActivated = false;
            thrusterDisabled = true;
        }

        if (_thrusterActivated && !thrusterDisabled)
        {
            _thruster.SetActive(true);
            _currentThrusterFuel -= 1.5f * Time.deltaTime;
            if(_currentThrusterFuel < 0)
            {
                _currentThrusterFuel = 0;
                _thruster.SetActive(false);
                thrusterDisabled = true;
            }
            
        }
        else if (!_thrusterActivated || thrusterDisabled)
        {
            _thruster.SetActive(false);
            if (_currentThrusterFuel != _maxThrusterFuel)
            {
                _currentThrusterFuel += 0.5f * Time.deltaTime;
                
                // so that you can't have more fuel then max
                if (_currentThrusterFuel > _maxThrusterFuel)
                {
                    _currentThrusterFuel = _maxThrusterFuel;
                }
            }
        }

        // cooldown until back at full
        if (_currentThrusterFuel == _maxThrusterFuel)
        {
            thrusterDisabled = false;
        }
        _uiManager.UpdateThrusterBar(_currentThrusterFuel, _maxThrusterFuel);

    }


    void CalculateMovement()
    {
        if (_thrusterActivated)
        {   
            _actualSpeed = _thrusterBoost + _setSpeed;
        }
        else
        {
            _actualSpeed = _setSpeed;
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * Time.deltaTime * _actualSpeed);        

        // using Clamping to set boundries (will not work when wrapping as done with the X axis)
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.5f, 2.5f));

        if (transform.position.x >= 11.5f)
        {
            transform.position = new Vector3(-11.5f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.5f)
        {
            transform.position = new Vector3(11.5f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_hasTripleShot)
        {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0.5f, 0.82f, 0), Quaternion.identity);
        }
        else if (_hasHomingShot)
        {
            GameObject target = _spawnManager.RandomEnemySelect();
            if (target) //if target is found shoot homing, if not disable homing
            {
                Laser homingLaser = Instantiate(_homingShotPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity).GetComponent<Laser>();
                homingLaser.AssignHoming(target);
            }
            else
            {
                _hasHomingShot = false;
            }
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
        }
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 3.0f;
        _audioSource.Play();
        AmmoHandler();
    }

    public void Damage()
    {
        if (_hasShield)
        {
            switch(_shieldStrength)
            {
                case 1:
                    _hasShield = false;
                    _shieldStrength = 0;
                    //Destroy(_playerShield.gameObject);
                    _shieldVisualizer.SetActive(false);
                    break;
                case 2:
                    _shieldStrength--;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                case 3:
                    _shieldStrength--;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;
                default:
                    Debug.Log("Invalid");
                    break;
            }
            return;
        }


        _lives--; //_lives -= 1;
        if (_lives < 0)
        {
            _lives = 0;
        }

        _uiManager.UpdateLives(_lives);
        if (_lives < 3 && _lives > 0)
        {
            ShowDamage();
        }


        if (_lives <= 0)
        {
            _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            if (_spawnManager)
            {
                _spawnManager.OnPlayerDeath();
            }
            // explosion here or in the player hurt visualizer
            Destroy(this.gameObject);
        }
        StartCoroutine(_uiManager.CameraShake());
    }

    private void ShowDamage()
    {
        int showDamage = Random.Range(0, 2);
        switch (_previousDamage)
        {
            case -1:
                _playerHurtVisualizer[showDamage].SetActive(true);
                break;
            case 0:
                _playerHurtVisualizer[1].SetActive(true); //right wing
                break;
            case 1:
                _playerHurtVisualizer[0].SetActive(true); //left wing
                break;
            default:
                Debug.Log("No damage to apply");
                break;
        }
        _previousDamage = showDamage;
    }

    private void FixDamage()
    {
        bool leftWingDamaged = _playerHurtVisualizer[0].activeSelf;
        bool rightWingDamaged = _playerHurtVisualizer[1].activeSelf;
        if (leftWingDamaged)
        {
            _playerHurtVisualizer[0].SetActive(false);
            return;
        }
        else if(rightWingDamaged)
        {
            _playerHurtVisualizer[1].SetActive(false);
            return;
        }
    }

    public void Powerup(int powerupID = 0)
    {
        switch (powerupID)
        {
            case 0:
                _hasTripleShot = true;
                break;
            case 1:
                _hasSpeedBoost = true;
                _setSpeed *= _speedMultiplier;
                break;
            case 2:
                _hasShield = true;
                _shieldStrength = 3;
                //_playerShield = Instantiate(_playerShieldPrefab, transform.position, Quaternion.identity);
                //_playerShield.transform.parent = this.transform;

                // this just actives the GameObject Shields placed under the Player transform (checks / unchecks it from view)
                // probably a better solution than actually instantiating and then destroying gameobjects using a prefab
                _shieldVisualizer.SetActive(true);
                _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.white;
                break;
            case 3:
                _hasHomingShot = true;
                // could add ammo as well
                // on completion random enemy explodes
                break;

            case 4: // health boost
                HealthBoostHandler();
                break;
            case 5: // add 10 to ammo
                AmmoHandler(10);
                break;
            default:
                Debug.Log("Power Up::Default value");
                break;
        }
        StartCoroutine(PowerupPowerdownRoutine(powerupID));
    }

    private IEnumerator PowerupPowerdownRoutine(int powerupID)
    {
        yield return new WaitForSeconds(5.0f);
        switch (powerupID)
        {
            case 0:
                _hasTripleShot = false;
                break;
            case 1:
                _hasSpeedBoost = false;
                _setSpeed /= _speedMultiplier;
                break;
            case 3:
                _spawnManager.RandomEnemySelfDestruct();
                AddToScore(100);
                break;
        }
    }

    public void AddToScore(int points = 0)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    void AmmoHandler(int updateAmmoCount = -1)
    {
        if (_ammoCount < 0)
        {
            _ammoCount = 0;
        }
        else
        {
            _ammoCount += updateAmmoCount;
            if (_ammoCount > _maxAmmo)
            {
                _ammoCount = _maxAmmo;
            }
        }

        _uiManager.UpdateAmmo(_ammoCount, _maxAmmo);
    }

    void HealthBoostHandler()
    {
        if (_lives == 3)
        {
            // Do nothing
        }
        else
        {
            _lives++;
            FixDamage();
            _uiManager.UpdateLives(_lives);
        }
    }
}
