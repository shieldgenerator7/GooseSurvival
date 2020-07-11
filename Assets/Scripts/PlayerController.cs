using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float maxMoveSpeed = 3;
    public float maxJumpFactor = 2;
    public float jumpDuration = 1;
    public float groundTestDistance = 0.1f;

    private float _speedFactor = 1;
    public float speedFactor
    {//1 = normal, 0.5 = half speed, 2 = double speed
        get => _speedFactor;
        set
        {
            moveSpeed /= _speedFactor;
            _speedFactor = Mathf.Clamp(value, 0.1f, 100);
            moveSpeed *= _speedFactor;
        }
    }

    private float moveSpeed = 0;

    private float jumpFactor = 0;
    private float jumpStartTime = 0;

    public bool Grounded
    {
        get
        {
            RaycastHit2D[] rch2ds = new RaycastHit2D[10];
            int count = coll2d.Cast(Vector2.down, rch2ds, groundTestDistance);
            for (int i = 0; i < count; i++)
            {
                Collider2D coll = rch2ds[i].collider;
                GameObject go = coll.gameObject;
                if (!coll.isTrigger && go != gameObject)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public int FacingDirection
        => (int)Mathf.Sign(transform.localScale.x);

    private Rigidbody2D rb2d;
    private Collider2D coll2d;
    private Animator animator;
    private PlayerControls playerControls;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        coll2d = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();
        //playerControls.PlayerMap.Move.performed += processMovement;
        playerControls.PlayerMap.Jump.performed +=
            iacbc => processJump(true);
        playerControls.PlayerMap.Jump.canceled +=
            iacbc => processJump(false);
        playerControls.Enable();
        jumpStartTime = -jumpDuration;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        //Flip sprite left or right
        if (rb2d.velocity.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(rb2d.velocity.x);
            transform.localScale = scale;
        }
        else if (Grounded)
        {
            processMovement(Vector2.right * transform.localScale.x * -1);
        }
    }

    void FixedUpdate()
    {
        //Movement
        Vector2 moveDir = rb2d.velocity;
        moveDir.x = moveSpeed;
        rb2d.velocity = moveDir;
        //Jump
        if (Time.time <= jumpStartTime + jumpDuration)
        {
            rb2d.AddForce(Physics2D.gravity * -1 * jumpFactor * rb2d.mass);
            jumpFactor = maxJumpFactor - maxJumpFactor * Mathf.Pow((Time.time - jumpStartTime) / jumpDuration, 2);
        }
    }

    void processMovement(InputAction.CallbackContext obj)
    {
        Vector2 inputDir = obj.ReadValue<Vector2>();

        processMovement(inputDir);
    }

    void processMovement(Vector2 inputDir)
    {
        //Movement Input
        moveSpeed = inputDir.x * maxMoveSpeed * speedFactor;
    }

    void processJump(bool jump)
    {
        //Jump Input
        if (jump)
        {
            if (jumpStartTime <= 0 && Grounded)
            {
                jumpStartTime = Time.time;
                jumpFactor = maxJumpFactor;
            }
        }
        else
        {
            jumpStartTime = 0;
            //Stop moving upward
            if (rb2d.velocity.y > 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.GetContact(0).point.y > transform.position.y + 0.1f)
        {
            Vector2 dir = Vector2.right * transform.localScale.x * -1;
            processMovement(dir);
        }
        else
        {
            if (rb2d.velocity.x == 0)
            {
                Vector2 dir = Vector2.right * transform.localScale.x;
                processMovement(dir);
            }
        }
    }

}
