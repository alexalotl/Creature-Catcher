using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CharacterController : PhysicsObject {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    public bool lastFlipSprite = false;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private GameObject game;
    private LevelManager level;
    private bool willDestroy;
    public AudioSource jump;
    public AudioSource creatureGet;
    public LevelManager levelManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        game = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Creature")
        {
            CreatureManager creatureScript = other.GetComponent<CreatureManager>();
            if (creatureScript.Collect()){
                creatureGet.Play();
            }
        }
    }

    protected override void ComputeVelocity()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("Main Menu");
        }

        Vector2 move = Vector2.zero;
        level = game.GetComponent<LevelManager>();
        
        move.x = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump.Play();
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonDown("Jump") && !grounded)
        {
            jump.Play();
            velocity.y = jumpTakeOffSpeed / 1.4f;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        if (Input.GetButtonDown("FireUp") || Input.GetButtonDown("FireDown") || Input.GetButtonDown("FireLeft") || Input.GetButtonDown("FireRight"))
        {
            Vector3 direction = new Vector3(0, 0, 0);
            if (Input.GetButtonDown("FireUp"))
            {
                direction += new Vector3(0, 1f, 0);
            }
            if (Input.GetButtonDown("FireDown"))
            {
                direction += new Vector3(0, -2f, 0);
            }
            if (Input.GetButtonDown("FireLeft"))
            {
                direction += new Vector3(-1, 0, 0);
            }
            if (Input.GetButtonDown("FireRight"))
            {
                direction += new Vector3(1, 0, 0);
            }
            jump.Play();
            DestroyBlock(direction);
        }

        void DestroyBlock(Vector3 direction)
        {
            levelManager.DestroyBlock(Vector3Int.FloorToInt(transform.position) + Vector3Int.RoundToInt(direction));
        }
        // animations
        //if (velocity.x == 50000000.0f)// fix this
        //{
        //    animator.Play("Run");
        //}
        //else
        //{
        //    animator.Play("Idle");
        //}
        //bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
        bool flipSprite = (move.x < 0.0f);
        if (flipSprite)
        {
            if (lastFlipSprite == false)
            {
                lastFlipSprite = true;
                flipSprite = true;
            }
            else
            {
                lastFlipSprite = true;
                flipSprite = false;
            }
        } else
        {
            if (move.x != 0.0f)
            {
                if (lastFlipSprite == true)
                {
                    lastFlipSprite = false;
                    flipSprite = true;
                }
                else
                {
                    lastFlipSprite = false;
                    flipSprite = false;
                }
            }
        }
        if (flipSprite)
        {
            //spriteRenderer.flipX = !spriteRenderer.flipX;
            transform.Rotate(0.0f, 180.0f, 0.0f);
            if (move.x < 0.0f)
            {
                transform.Translate(0.1f, 0.0f, 0.0f);
            } else if (move.x > 0.0f)
            {
                transform.Translate(0.1f, 0.0f, 0.0f);
            }
        }
        //animator.SetBool("grounded", grounded);
        //animator.SetFloat("velocityx", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }
}

