using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _speed = 15.0f;
    [SerializeField]
    private GameObject _explosionPrefab = null;
    private SpawnManager _spawnManager = null;
    private WaveManager _waveManager = null;

    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (!_spawnManager)
        {
            Debug.Log("Spawn Manager is Null");
        }
        _waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
        if (!_waveManager)
        {
            Debug.LogError("Wave Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {   
        transform.Rotate(Vector3.forward * Time.deltaTime * _speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            //_spawnManager.StartSpawning(); // Pre wave implementation
            _waveManager.StartWaves();
            Destroy(this.gameObject, 1f);
        }
    }
}
