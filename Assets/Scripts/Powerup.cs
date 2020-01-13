using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.5f;
    [SerializeField]
    private int _PowerupID = 0; // 0 = Triple Shot, 1 = Speed, 2 = Shield
    [SerializeField]
    private AudioClip _clip;

    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (transform.position.y < -8)
        {
            Destroy(this.gameObject);
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
    }
}
