using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Enemy : MonoBehaviour
{

    public enum State : int { Idle, Patrul, Atack};
    public State EnemyState = State.Patrul;

    [Header("Healf")]
    bool dead;
    public int HealfPoint = 3;

    [Header("Move")]
    [SerializeField] private float moveSpeed;

    [Header("Cheking")]
    [SerializeField] private float offcetCheckGround;
    [SerializeField] private float directionCheckGround;
    [SerializeField] private float directionCheckWall;
    [SerializeField] private float distanceForWall;
    [SerializeField] private float distanceForGroundCheck;
    [SerializeField] private bool stopEnemy;

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask PlayerMask;
    public Transform Target;

    [Header("Atack")]
    [SerializeField] private bool IsNotWalls;
    [SerializeField] private bool closeAtack;
    [SerializeField] private bool rangedAtack;
    [SerializeField] private float minDistanceForTarget;
    [SerializeField] private float minDistanceForTargetRange;
    [SerializeField] private float waitTimeAtack;
    [SerializeField] private float waitTimeAtackRange;
    [SerializeField] private float timeAtack;

    [SerializeField] private float timeShot;
    [SerializeField] private float startTime;

    [SerializeField] private Transform BulletSpawn;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject Pistol;
    [SerializeField] private GameObject Light;

    Animator animator;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        EnemyState = State.Patrul;
    }


    void Update()
    {
        
        if(HealfPoint <= 0 && !dead)
        {
            Dead();
            dead = true;
        }

        //CheckPatrul
        if (((GroundCheck() && WallCheck()) || (!GroundCheck() && !WallCheck()) || (!GroundCheck() && WallCheck())) && !stopEnemy)
        {
            StartCoroutine(WaitPartur());
        }

        //Patrul
        if (EnemyState == State.Patrul)
        {
            rb.velocity = new Vector2(moveSpeed * transform.lossyScale.x, rb.velocity.y);
            animator.SetBool("Walk", true);
        } else
        {
            animator.SetBool("Walk", false);
        } 
        
        if (EnemyState == State.Atack)
        {
            animator.SetBool("Run", true);
        } else
        {
            animator.SetBool("Run", false);
        }



        //Atack
        if(EnemyState == State.Atack)
        {

            if (transform.position.x > Target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (transform.position.x < Target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            if (closeAtack)
            {
               
                float distanceForTarget = Vector3.Distance(transform.position, Target.position);
                if(distanceForTarget < minDistanceForTarget)
                {
                    timeAtack -= Time.deltaTime;
                    if(timeAtack <= 0)
                    {
                        StartCoroutine(Atack());
                        timeAtack = 0.2f;
                    }

                } else
                {
                    rb.velocity = new Vector2(moveSpeed * 5f * transform.localScale.x, rb.velocity.y);
                }
            }
            
            if (rangedAtack)
            {
                float distanceForTarget = Vector3.Distance(transform.position, Target.position);
                

                if (distanceForTarget <= minDistanceForTargetRange)
                {
                    StartCoroutine(AtackRange());
                } else
                {
                    rb.velocity = new Vector2(moveSpeed * 5f * transform.localScale.x, rb.velocity.y);
                }
            }

        }

        if (Target != null)
        {
            Physics2D.queriesHitTriggers = false;
            RaycastHit2D checkwalls = Physics2D.Raycast(transform.position, Target.position - transform.position, 999f, PlayerMask);



            if (checkwalls.transform.root != null)
            {
                Debug.DrawLine(transform.position, Target.position, Color.blue, 999f);
                Debug.Log(checkwalls.transform.root.name);

                if (checkwalls.transform.GetComponentInChildren<PlayerController>())
                {
                    Debug.Log("Включил атаку");
                    EnemyState = State.Atack;
                }
            }
        }
        else return;


    }

    private IEnumerator Atack()
    {
        yield return new WaitForSeconds(waitTimeAtack);
        if (Target != null)
        {
            Target.GetComponent<PlayerController>().HealfPoint -= 1;
        } 
        else yield return null;
    }

    private IEnumerator AtackRange()
    {
        yield return new WaitForSeconds(waitTimeAtackRange);

        if (timeShot <= 0)
        {
            timeShot = startTime;
            Vector3 targetPoint = Target.transform.position - transform.position;
            float rotation = Mathf.Atan2(targetPoint.y, targetPoint.x) * Mathf.Rad2Deg;
            BulletSpawn.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
            GameObject i;
            i = Instantiate(Bullet, BulletSpawn.transform.position, BulletSpawn.transform.rotation);
        }else
        {
            timeShot -= Time.deltaTime;
        }

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    private IEnumerator WaitPartur()
    {
        int stopTime = Random.Range(2, 5);
        stopEnemy = true;
        EnemyState = State.Idle;
        yield return new WaitForSeconds(stopTime);
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
        EnemyState = State.Patrul;
        stopEnemy = false;
    }

    private bool GroundCheck()
    {
        RaycastHit2D ground = Physics2D.Raycast(transform.position + new Vector3(offcetCheckGround * transform.localScale.x, 0, 0), Vector2.down, directionCheckGround, groundLayerMask);
        Debug.DrawLine(transform.position + new Vector3(offcetCheckGround * transform.localScale.x, 0, 0), ground.point);
        distanceForGroundCheck = Vector3.Distance(transform.position, ground.point);
        return ground.collider != null;
    }
    private bool WallCheck()
    {
        RaycastHit2D wall = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, directionCheckWall, groundLayerMask);
        Debug.DrawLine(transform.position, wall.point);
        distanceForWall = Vector3.Distance(transform.position, wall.point);
        return wall.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.transform.tag == "Player")
        {
            Target = target.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D target)
    {
        if (target.transform.tag == "Player")
        {
            Target = null;
            StopAllCoroutines();
            StartCoroutine(WaitPartur());
        }
    }

    void Dead()
    {
        if(HealfPoint <= 0)
        {
            EnemyState = State.Idle;
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("Dead", true);
            transform.tag = "Ground";
            transform.gameObject.layer = 6;

            Light.SetActive(false);

            if(Pistol != null)
            Pistol.SetActive(false);

            transform.GetComponent<Enemy>().enabled = false;
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            transform.GetComponent<Collider2D>().enabled = false;
        }

    }

}
