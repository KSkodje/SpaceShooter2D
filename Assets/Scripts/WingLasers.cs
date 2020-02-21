using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingLasers : MonoBehaviour
{

    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            player.Damage();

            Boss boss = GetComponentInParent<Boss>();
            boss.WingLasersToggle();
        }
    }
}
