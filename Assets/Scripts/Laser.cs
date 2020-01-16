using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    private bool _isEnemyFire = false;
    private bool _homing = false;

    private GameObject _enemyTarget = null;

    void Update() {
        if (_isEnemyFire)
        {
            MoveDown();
        }
        else if (_homing)
        {
            HomingLaser();
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

    void HomingLaser()
    {

        Transform _enemy = _enemyTarget.GetComponent<Transform>();
        if (_enemy)
        {
            Vector3 targetPosition = _enemy.position - transform.position;
            transform.Translate(targetPosition * Time.deltaTime * 1.5f);

            if (!_enemy)
            {
                Destroy(this.gameObject);
            }
            float shotAliveDuration = 3.0f; // shots self destruct after 3 seconds
            Destroy(this.gameObject, shotAliveDuration);
        }
        else
        {
            _homing = false;
        }
    }

    public void AssignEnemyLasers()
    {
        _isEnemyFire = true;
    }

    public void AssignHoming(GameObject targetEnemy)
    {
        _enemyTarget = targetEnemy;
        _homing = true;
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
