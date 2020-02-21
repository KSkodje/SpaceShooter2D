using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    private bool _isEnemyFire = false;
    private bool _backwardsFire = false;
    private bool _homing = false;
    private bool _sideways = false;
    private char _dir;
    private float _dirOffset = 0;

    private GameObject _enemyTarget = null;

    private void Start()
    {
        if (_backwardsFire)
        {
            if (_sideways)
            {
                transform.position = new Vector3(transform.position.x + _dirOffset, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 2.8f, transform.position.z);
            }
            _speed /= 2f;
        }
    }

    void Update() {
        if (_isEnemyFire)
        {
            if (_backwardsFire)
            {
                MoveUp();
            }
            else if (_sideways)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
                if (_dir == 'r')
                {
                    MoveDown();
                }
                if(_dir == 'l')
                {
                    MoveUp();
                }
            }
            else
            {
                MoveDown();
            }
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
        DestroyLasers();
    }
    void MoveDown()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        DestroyLasers();
    }

    void DestroyLasers()
    {
        float posX = transform.position.x;
        float posY = transform.position.y;
        if (posX < -11f || posX > 11f || posY < -8 || posY > 8)
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

    public void EnemyBackwardsFire()
    {
        _isEnemyFire = true;
        _backwardsFire = true;
    }

    public void SidewaysEnemy(char direction)
    {
        _isEnemyFire = true;
        _sideways = true;
        _dir = direction;
    }

    public void SidewaysAndBackwardsFire(char direction)
    {
        _dir = direction;
        _isEnemyFire = true;
        _sideways = true;
        _backwardsFire = true;
        if (direction == 'l')
        {
            _dirOffset = 2.8f;
        }
        else if(direction == 'r')
        {
            _dirOffset = -2.8f;
        }
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
            if (transform.parent)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

}
