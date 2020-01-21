using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.0f;

    private Player _player = null;
    private Animator _anim = null;

    [SerializeField]
    private GameObject _enemyFirePrefab = null;
    private float _fireRate = 3.0f;
    private float _canFire = -1.0f;
    private bool _dying = false;

    [SerializeField]
    private AudioClip _explosionSound = null;
    private AudioSource _audioSource;


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
    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire && !_dying)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_enemyFirePrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i=0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLasers();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);

        if (transform.position.y < -6.0f)
        {
            float randXpos = Random.Range(-9f, 9f);
            transform.position = new Vector3(randXpos, 7, 0);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        string hitBy = other.tag;
        if (hitBy == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player)
            {
                player.Damage();
            }
            OnEnemyDeath();
        }

        if (hitBy == "Laser")
        {
            //_audioSource.Play();
            Destroy(this.GetComponent<BoxCollider2D>()); // to enable the laser to pass through the explosion. Just deactivating isTrigger did not seem to do it.
            Destroy(other.gameObject);
            if (_player)
            {
                _player.AddToScore(10);
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
        _dying = true; // prevents it from being able to fire if death animation is triggered
        _audioSource.Play();
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        Destroy(this.gameObject, 2.37f);
    }
}
