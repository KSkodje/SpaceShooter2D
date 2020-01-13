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

    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (!_spawnManager)
        {
            Debug.Log("Spawn Manager is Null");
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
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 1f);
        }
    }
}
