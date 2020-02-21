using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private GameObject[] enemies = null;
    private Transform bestTarget = null;
    [SerializeField]
    private float _homingSpeed = 5f;
    [SerializeField]
    private float _rotateSpeed = 100f;

    private Rigidbody2D rb = null;
    [SerializeField]
    private GameObject _explosionPrefab = null;
    
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();

        Target();
    }

    void Update()
    {
        HomeOnTarget();
    }

    void Target()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            _explosionPrefab.transform.localScale = new Vector3(.2f, .2f, .2f);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        FindClosestTarget();
    }

    void HomeOnTarget()
    {
        //https://github.com/Brackeys/Homing-Missile/blob/master/Homing%20Missile/Assets/HomingMissile.cs
        if (bestTarget == null)
        {
            Target();
        }
        transform.position = Vector3.MoveTowards(transform.position, bestTarget.position, Time.deltaTime * _homingSpeed);
        Vector3 direction = bestTarget.position - transform.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -rotateAmount * _rotateSpeed;
        rb.velocity = transform.up * _homingSpeed * Time.deltaTime;
    }

    void FindClosestTarget()
    {
        //https://forum.unity.com/threads/clean-est-way-to-find-nearest-object-of-many-c.44315/
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 playerPos = transform.position;
        if (enemies.Length > 0)
        {
            foreach (GameObject potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - playerPos;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
        }
        else
        {
            bestTarget = null;
        }
    }
}
