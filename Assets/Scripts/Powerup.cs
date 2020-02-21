using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.5f;
    [SerializeField]
    private int _PowerupID = 0; // 0 = Triple Shot, 1 = Speed, 2 = Shield, 3 = Homing Shot, 4 = Homing Missile, 5 = Health, 6 = Ammo, 7 = Negative Pickup
    /*
     * PowerupID number is for handling the logic
     * 
     * In the spawn manager it is the list order that is used to spawn the object
     */
    [SerializeField]
    private AudioClip _clip = null;

    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (transform.position.y < -8)
        {
            Destroy(this.gameObject);
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.position, Time.deltaTime * (_speed * 1.1f));
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string hitBy = collision.tag;
        if (hitBy == "Player")
        {
            Player _player = collision.transform.GetComponent<Player>();
            if (_player) // nullcheck
            {
                _player.Powerup(_PowerupID);
            }
            AudioSource.PlayClipAtPoint(_clip, new Vector3(0, 0, 1));
            Destroy(this.gameObject);
        }
        if (hitBy == "Enemy_Laser")
        {
            Destroy(this.gameObject);
        }
    }
}
