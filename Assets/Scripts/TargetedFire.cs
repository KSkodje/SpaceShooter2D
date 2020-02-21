using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedFire : MonoBehaviour
{
    [SerializeField]
    private Player _player = null;
    private Vector3 direction;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player)
        {
            direction = _player.transform.position - transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(direction != null)
        {
            transform.Translate(direction * Time.deltaTime * .5f, Space.World); // without space.world it changes direction based on the rotation
            transform.Rotate(Vector3.forward);
        }
        if(transform.position.x >= 11f || transform.position.x <= -11f || transform.position.y >= 8f || transform.position.y <= -8f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            _player.Damage();
            Destroy(this.gameObject);
        }
    }
}
