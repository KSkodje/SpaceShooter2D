using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // SerializeField here used to have the field visible in the Unity Editor Inspector (private variables otherwise does not show)
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private bool _hasTripleShot = false;
    private bool _hasSpeedBoost = false;
    private bool _hasShield = false;

    // Prefabs and links
    [SerializeField]
    private GameObject _laserPrefab = null;
    [SerializeField]
    private GameObject _tripleShotPrefab = null;
    //[SerializeField]
    //private GameObject _playerShieldPrefab = null;
    [SerializeField]
    private SpawnManager _spawnManager = null;
    [SerializeField]
    private UIManager _uiManager = null;
    [SerializeField]
    private GameObject _shieldVisualizer = null;
    [SerializeField]
    private GameObject[] _playerHurtVisualizer = null;
    [SerializeField]
    private GameObject _thruster = null;
    [SerializeField]
    private int _score = 0;
    private int _previousDamage = -1;

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
        _thruster.SetActive(true);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * Time.deltaTime * _speed);        

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
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
        }
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 3.0f;
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_hasShield)
        {
            _hasShield = false;
            //Destroy(_playerShield.gameObject);
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--; //_lives -= 1;
        _uiManager.UpdateLives(_lives);
        if (_lives < 3 && _lives > 0)
        {
            int showDamage = Random.Range(0, 2);
            switch (_previousDamage)
            {
                case -1:
                    _playerHurtVisualizer[showDamage].SetActive(true);
                    break;
                case 0:
                    _playerHurtVisualizer[1].SetActive(true);
                    break;
                case 1:
                    _playerHurtVisualizer[0].SetActive(true);
                    break;
                default:
                    Debug.Log("No damage to apply");
                    break;
            }
            _previousDamage = showDamage;
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
                _speed *= _speedMultiplier;
                break;
            case 2:
                _hasShield = true;
                //_playerShield = Instantiate(_playerShieldPrefab, transform.position, Quaternion.identity);
                //_playerShield.transform.parent = this.transform;

                // this just actives the GameObject Shields placed under the Player transform (checks / unchecks it from view)
                // probably a better solution than actually instantiating and then destroying gameobjects using a prefab
                _shieldVisualizer.SetActive(true);
                break;
            default:
                Debug.Log("Default value");
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
                _speed /= _speedMultiplier;
                break;
        }
    }

    public void AddToScore(int points = 0)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

}
