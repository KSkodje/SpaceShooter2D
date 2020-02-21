using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShot : MonoBehaviour
{
    [SerializeField] private float _GrowthRate;
    private bool _doneCharging = false;

    [SerializeField] private Player _player = null;
    private Vector3 direction;

    [SerializeField] private float _selfDestructTimer = 10f;
    void Start()
    {

        transform.localScale = new Vector3(.1f, .1f, .1f);
        StartCoroutine(Charge());
        _GrowthRate = Time.deltaTime * 1f;
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    IEnumerator Charge()
    {
        while (transform.localScale.x < .5f)
        {
            transform.localScale += new Vector3(+_GrowthRate, +_GrowthRate, +_GrowthRate);
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(1f);
        direction = _player.transform.position - transform.position;
        _doneCharging = true;
        _selfDestructTimer = Time.time + _selfDestructTimer;
    }

    private void Update()
    {
        HomeOnPlayer();
        if (Time.time > _selfDestructTimer && _doneCharging)
        {
            SelfDestruct();
        }

    }
    void HomeOnPlayer()
    {
        if (_doneCharging)
        {
            if (direction != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Time.deltaTime * 1.5f);
                direction.Normalize();
            }
        }
        if (transform.position.x >= 11f || transform.position.x <= -11f || transform.position.y >= 8f || transform.position.y <= -8f)
        {
            Destroy(this.gameObject);
        }
    }

    void SelfDestruct()
    {
        if (transform.parent)
        {
            Destroy(this.transform.parent.gameObject);
        }
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player.Damage();
            SelfDestruct();
        }
        if (collision.tag == "Boss")
        {
            SelfDestruct();
        }
    }
}
