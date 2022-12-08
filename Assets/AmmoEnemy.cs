using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AmmoEnemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float ForceDamage = 2;
    [SerializeField] private int DAMAGE;
    [SerializeField] private LayerMask AmmoLayerMask;
    public Transform Target;


    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        Physics2D.queriesHitTriggers = false;
        RaycastHit2D checkTarget;
        if(checkTarget = Physics2D.Raycast(transform.position, transform.right, 0.1f, AmmoLayerMask))
        {
            if (checkTarget.transform != null)
            {
                if(checkTarget.transform.tag == "Player" || checkTarget.transform.tag == "Enemy")
                {
                    if (checkTarget.transform.tag == "Player")
                    {
                        if (checkTarget.transform.GetComponent<PlayerController>().HealfPoint > 0)
                        {
                            checkTarget.transform.GetComponent<PlayerController>().HealfPoint -= DAMAGE;
                            checkTarget.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.right.normalized * checkTarget.transform.localScale.x * ForceDamage, ForceMode2D.Impulse);
                        }
                        Destroy(gameObject);
                    }
                    if (checkTarget.transform.tag == "Enemy")
                    {
                        if (checkTarget.transform.GetComponent<Enemy>().HealfPoint > 0)
                        {
                            checkTarget.transform.GetComponent<Enemy>().HealfPoint -= DAMAGE;
                            checkTarget.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.right.normalized * checkTarget.transform.localScale.x * ForceDamage, ForceMode2D.Impulse);
                        }
                        Destroy(gameObject);
                    }

                } else
                {
                    Destroy(gameObject);
                }
                
            }
        }
    }

}
