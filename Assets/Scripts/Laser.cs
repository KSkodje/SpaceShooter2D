using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    private bool _isEnemyFire = false;

    void Update() {
        if (_isEnemyFire)
        {
            MoveDown();
        }
        else
        {
            MoveUp();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _speed);

        if (transform.position.y > 8f)
        {
            if (transform.parent)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    void MoveDown()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);

        if (transform.position.y < -7f)
        {
            if (transform.parent)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLasers()
    {
        _isEnemyFire = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && _isEnemyFire)
        {
            Player player = collision.GetComponent<Player>();
            if (player)
            {
                player.Damage();
            }
        }
    }
}
