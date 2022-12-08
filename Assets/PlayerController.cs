using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State: int { Idle, Run, Jump, Fly, CrawlWall, CrawlCeil};
    public State PlayerState = State.Idle; 

    [Header("Healf")]
    public int HealfPoint = 3;
    [SerializeField] private int DamageShuriken = 1;
    [SerializeField] private int Damage = 2;

    [SerializeField] private GameObject EnemyStayInTrigger;

    [Header("Move")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float coutchSpeed;
    [SerializeField] private float curentmoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float moveDir;
    [SerializeField] private bool playerFly = false;
    public bool canMove = true;

    [Header("GroundCheck")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("CrowWall")]
    [SerializeField] private bool crawlWall;
    [SerializeField] private float distanceToWall;
    [SerializeField] private float slideForceWall;
    [SerializeField] private float jumpForceWall;
    [SerializeField] private float moveSpeedWall;
    [SerializeField] private float dirCheckWall;
    [SerializeField] private float timeWaitAfterJumpWall;

    [Header("CrowCeiling")]
    [SerializeField] private bool crawlCeil;
    [SerializeField] private float distanceToCeil;
    [SerializeField] private float slideForceCeil;
    [SerializeField] private float jumpForceCeil;
    [SerializeField] private float moveSpeedCeil;
    [SerializeField] private float offsetCeilCheck;

    [Header("Win/Lose")]
    public bool playerWin;
    public Transform Win;
    public bool playerLose;
    public Transform Lose;


    [Header("Atack")]
    [SerializeField] private float BulletSpeed;
    [SerializeField] private float offsetMousePoint;
    [SerializeField] public int Hurican = 5;
    public GameObject Bullet;
    public GameObject BulletSpawn;
    private Vector3 mousePos;
    GameObject i;


    //Components
    Animator animator;
    CircleCollider2D circleCollider2D;
    BoxCollider2D checkWallCollider;
    Rigidbody2D rb; 


    private void Start()
    {
        PlayerState = State.Idle;
        rb = transform.GetComponent<Rigidbody2D>();
        circleCollider2D = transform.GetComponent<CircleCollider2D>();
        checkWallCollider = transform.GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(HealfPoint <= 0)
        {
            Dead();
        }

        StartCoroutine(Losed());
        StartCoroutine(Wined());

        if (moveDir == 0 && PlayerState != State.CrawlWall && PlayerState != State.CrawlCeil && PlayerState != State.Jump && PlayerState != State.Run && PlayerState != State.Fly)
        {
            PlayerState = State.Idle;
            animator.SetInteger("Climp", 0);
        }

        if (PlayerState == State.Idle)
        {
        }

        if (PlayerState == State.Fly)
        {
        }

        //CheckMove
        if (canMove)
        {
            moveDir = Input.GetAxisRaw("Horizontal");
        } else
        {
            moveDir = 0;
        }

        if (moveDir != 0 && PlayerState != State.CrawlWall && PlayerState != State.CrawlCeil)
        {
            PlayerState = State.Run;
        }
        else if (moveDir == 0 && PlayerState != State.CrawlWall && PlayerState != State.CrawlCeil)
        {
            PlayerState = State.Idle;
        }

        //Run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.speed = 0.5f;
            curentmoveSpeed = moveSpeed / 3;
        }
        else
        {
            curentmoveSpeed = moveSpeed;
            animator.speed = 1f;
        }


        if (PlayerState == State.Run && PlayerState != State.Jump && PlayerState != State.CrawlWall)
        {
            rb.velocity = new Vector2(moveDir * curentmoveSpeed, rb.velocity.y);
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        //CheckJump
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && PlayerState != State.Jump && isGrounded)
        {
            PlayerState = State.Jump;
        }

        //Jump
        if (PlayerState == State.Jump && canMove)
        {
            Vector2 JumpDir = new Vector2(5f * moveDir, jumpForce);
            rb.velocity = JumpDir;
            animator.SetBool("isGrounded", false);
        }
        else
        {
            animator.SetBool("isGrounded", true);
        }

        //CheckCrawlWall
        if ((CheckWall1() || CheckWall2() && playerFly) || (CheckWall1() || CheckWall2() && PlayerState == State.CrawlCeil) && canMove)
        {
            PlayerState = State.CrawlWall;
            animator.SetInteger("Climp", 1);
        }

        //CrawlWall
        if (PlayerState == State.CrawlWall && canMove)
        {
            if (rb.velocity.y < slideForceWall && rb.velocity.y != -moveSpeedWall && playerFly)
            {
                rb.velocity = new Vector2(0f, slideForceWall);
            }

            if (Input.GetKey(KeyCode.W))
            {
                rb.velocity = new Vector2(0f, moveSpeedWall);
                animator.SetInteger("Climp", 2);
            }

            if (Input.GetKey(KeyCode.S))
            {
                rb.velocity = new Vector2(0f, -moveSpeedWall);
                animator.SetInteger("Climp", -2);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //StopCoroutine("TimeWaitAfterJumpWall");
                StartCoroutine(TimeWaitAfterJumpWall());
                Vector2 JumpDir = new Vector2(5 * moveDir, jumpForceWall);
                rb.velocity = JumpDir;
                PlayerState = State.Fly;
                animator.SetInteger("Climp", 0);
            }
        }

        //CheckCrawCeil
        if (CheckCrawCeil() && (!Input.GetKey(KeyCode.Space) || !Input.GetKey(KeyCode.S)))
        {
            PlayerState = State.CrawlCeil;
            //animator.SetInteger("Climp", 0);
        }

        //CrawCeil
        if (PlayerState == State.CrawlCeil && playerFly && canMove)
        {
            if (rb.velocity.y < slideForceCeil)
            {
                rb.velocity = new Vector2(0f, slideForceCeil);
            }

            if (Input.GetKey(KeyCode.A))
            {
                rb.velocity = new Vector2(-slideForceCeil, rb.velocity.y);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector2(slideForceCeil, rb.velocity.y);
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(TimeWaitAfterJumpWall());
                rb.velocity = Vector2.down * jumpForceCeil;
                PlayerState = State.Fly;
            }
        }


        //CheckFly
        if (!CheckWall1() && !CheckWall2() && !CheckCrawCeil() && playerFly && !isGrounded)
        {
            PlayerState = State.Fly;
            animator.SetBool("isGrounded", false);
            animator.SetInteger("Climp", 0);
        }
        else
        {
            animator.SetBool("isGrounded", true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(0);
        }

        //Atack
        if (Input.GetMouseButtonDown(0))
        {
            if(EnemyStayInTrigger != null)
            {
                EnemyStayInTrigger.gameObject.GetComponent<Enemy>().HealfPoint -= 1;
            }

            animator.SetBool("Atack", true);
        } else
        {
            animator.SetBool("Atack", false);
        }


        if (Input.GetMouseButtonDown(1))
        {
            if (Hurican > 0)
            {
                Hurican--;
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float rotation = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                BulletSpawn.transform.rotation = Quaternion.Euler(0f, 0f, rotation + offsetMousePoint);
                Instantiate(Bullet, BulletSpawn.transform.position, BulletSpawn.transform.rotation);
            }
            else return;

        }


        Animations();
        Flip();
        CanJump();
    
    }

    private IEnumerator TimeWaitAfterJumpWall()
    {
        playerFly = false;
        yield return new WaitForSeconds(timeWaitAfterJumpWall);
        playerFly = true;
    }

    void Flip()
    {
        if(PlayerState != State.CrawlWall && PlayerState != State.CrawlCeil)
            if(moveDir > 0)
            {transform.localScale = new Vector2(1, 1);}
            else if (moveDir < 0)
            {transform.localScale = new Vector2(-1, 1);}
    }

    void CanJump()
    {
        if (GroundCheck())
        {
            isGrounded = true;
            PlayerState = State.Idle;
            playerFly = false;
        }
        else
        {
            isGrounded = false;
            if(PlayerState != State.CrawlWall)
            playerFly = true;
        }
    }

    private bool CheckCrawCeil()
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D checkCeil = Physics2D.Raycast(transform.position + Vector3.up * offsetCeilCheck, transform.up, distanceToCeil, groundLayerMask);
        Debug.DrawLine(transform.position + Vector3.up * offsetCeilCheck, checkCeil.point);

        return checkCeil.collider != null;
    }

    private bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.CircleCast(circleCollider2D.bounds.center, circleCollider2D.radius, Vector2.down, 0.1f, groundLayerMask);
        return hit.collider != null;
    }

    void Animations()
    {
        if (moveDir != 0 && isGrounded)
        {
            animator.SetBool("Run", true);
        } else if(!isGrounded)
        {
            animator.SetBool("Run", false);
        }
    }


    private bool CheckWall1()
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D checkWall1 = Physics2D.BoxCast(checkWallCollider.bounds.center, checkWallCollider.bounds.size, 0f, Vector2.left, dirCheckWall, groundLayerMask);
        return checkWall1.collider != null;

    }

    private bool CheckWall2()
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D checkWall2 = Physics2D.BoxCast(checkWallCollider.bounds.center, checkWallCollider.bounds.size, 0f, Vector2.right, dirCheckWall, groundLayerMask);
        return checkWall2.collider != null;

    }

    IEnumerator Losed()
    {
        if (playerLose && !playerWin)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            canMove = false;
            Lose.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(0);
        }
    }
    
    IEnumerator Wined()
    {
        if (playerWin && !playerLose)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
            canMove = false;
            Win.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(0);
        }
    }

    void Dead()
    {
        if (HealfPoint <= 0)
        {
            playerLose = true;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.gameObject.CompareTag("Enemy") && !collision.isTrigger)
        {
            if(Vector2.Distance(transform.position, collision.gameObject.transform.position) < 2f)
            {
                EnemyStayInTrigger = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.gameObject.CompareTag("Enemy"))
        {
            EnemyStayInTrigger = null;
        }
    }
}
