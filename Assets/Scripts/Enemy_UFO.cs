using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_UFO : MonoBehaviour
{
    private Player _player = null;
    [SerializeField]
    private float _speed = 1.0f;
    [SerializeField]
    private float _rotationSpeed = 66.6f;
    private float _direction;
    private float _dirChange = 0;

    private float _fireRate = 4.5f;
    private float _canFire = 0f;


    [SerializeField]
    private GameObject _explosionPrefab = null;
    [SerializeField]
    private GameObject _enemyFirePrefab = null;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (!_player)
        {
            Debug.LogError("Player is null");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * _rotationSpeed);
        if (Time.time > _canFire)
        {
            TargetedShot();
        }
        CalculateMovement();

    }

    void CalculateMovement()
    {
        if (transform.position.x <= -10f)
        {
            _direction = Random.Range(1f, 3f);
            _dirChange = Time.time + Random.Range(.5f, 2.5f);
        }
        if (transform.position.x >= 10f)
        {
            _direction = Random.Range(-1f, -3f);
            _dirChange = Time.time + Random.Range(.5f, 2.5f);
        }
        if (transform.position.y < -7.5f)
        {
            float randXpos = Random.Range(-9f, 9f);
            transform.position = new Vector3(randXpos, 7, 0);
        }
        if (Time.time > _dirChange)
        {
            _direction = Random.Range(-3f, 3f);
            _dirChange = Time.time + Random.Range(.5f, 2.5f);
        }
        transform.Translate(new Vector3(_direction,-1,0) * Time.deltaTime * _speed, Space.World);
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        string hitBy = collision.tag;
        if (hitBy == "Player")
        {
            _player.Damage();
            OnEnemyDeath();
        }
        else if(hitBy == "Laser" || hitBy == "Missile")
        {
            _player.AddToScore(10);
            Destroy(collision.gameObject);
            OnEnemyDeath();
        }
        else if(hitBy == "Enemy Laser")
        {
            Destroy(collision.gameObject);
        }
    }

    void OnEnemyDeath()
    {
        _speed = 0;
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 1f);
    }

    void TargetedShot()
    {
        _canFire = Time.time + _fireRate;
        Instantiate(_enemyFirePrefab, transform.position, Quaternion.identity);
    }
}
