using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int DAMAGE = 2;
    [SerializeField] private int ForceDamage;
    [SerializeField] private LayerMask AmmoLayerMask;

    [SerializeField] private Transform Player;


    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (HitCheck())
        {
            Destroy(gameObject);
        }

    }


    private bool HitCheck()
    {
        Physics2D.queriesHitTriggers = false;
        RaycastHit2D checkCeil = Physics2D.Raycast(transform.position, transform.right, 0.1f, AmmoLayerMask);
        if(checkCeil == true)
        {
            if (checkCeil.transform.GetComponent<Enemy>())
            {
                if (checkCeil.transform.GetComponent<Enemy>().HealfPoint > 0)
                {
                    checkCeil.transform.GetComponent<Enemy>().Target = GameObject.FindGameObjectWithTag("Player").transform;
                    checkCeil.transform.GetComponent<Enemy>().EnemyState = Enemy.State.Atack;
                    checkCeil.transform.GetComponent<Enemy>().HealfPoint -= DAMAGE;
                    checkCeil.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.right.normalized * -checkCeil.transform.localScale.x * ForceDamage, ForceMode2D.Impulse);
                }
            }
            else Destroy(gameObject);
        }
        

        return checkCeil.transform != null;
    }
}
