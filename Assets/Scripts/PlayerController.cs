using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float maxMoveSpeed = 3;
    public float crouchMoveSpeed = 1;
    public float maxJumpFactor = 2;
    public float jumpDuration = 1;
    public float groundTestDistance = 0.1f;
    public float slamRadius = 5;
    public float slamForce = 10;
    public float boostForce = 2;
    public GameObject holdPosition;
    public GameObject stashPosition;

    private float _speedFactor = 1;
    public float speedFactor
    {//1 = normal, 0.5 = half speed, 2 = double speed
        get => _speedFactor;
        set
        {
            moveSpeed /= _speedFactor;
            _speedFactor = Mathf.Clamp(value, 0.1f, 10);
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

    private bool crouchingInput = false;
    public bool Crouching
    {
        get => animator.GetBool("crouching");
        set => animator.SetBool("crouching", value);
    }

    public int FacingDirection
        => (int)Mathf.Sign(transform.localScale.x);

    private Collectible _item;
    public Collectible item
    {
        get => _item;
        set
        {
            _item?.drop();
            _item = value;
            _item?.stashItem(false, this);
        }
    }
    private Collectible _stashedItem;
    public Collectible stashedItem
    {
        get => _stashedItem;
        set
        {
            _stashedItem = value;
            _stashedItem?.stashItem(true, this);
        }
    }

    public CheckPoint checkPoint;

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
        playerControls.PlayerMap.Move.performed += processMovement;
        playerControls.PlayerMap.Jump.performed +=
            iacbc => processJump(true);
        playerControls.PlayerMap.Jump.canceled +=
            iacbc => processJump(false);
        playerControls.PlayerMap.Throw.performed +=
            iacbc => processThrowItem(true);
        playerControls.PlayerMap.Stash.performed +=
            iacbc => processStashItem();
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

        //Movement Input
        moveSpeed = inputDir.x * maxMoveSpeed * speedFactor;

        //Crouch Input
        if (inputDir.y < 0)
        {
            if (Grounded)
            {
                moveSpeed = inputDir.x * crouchMoveSpeed * speedFactor;
            }
            else if (!Crouching)
            {
                //Upwards boost
                if (rb2d.velocity.y > 0)
                {
                    rb2d.AddForce(Vector2.up * boostForce * rb2d.mass);
                }
            }
            crouchingInput = true;
            Crouching = true;
        }
        else
        {
            crouchingInput = false;
            if (Grounded)
            {
                Crouching = false;
            }
        }
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

    void processThrowItem(bool throwItem)
    {
        if (item && item is Weapon)
        {
            ((Weapon)item).throwWeapon(Vector2.right * FacingDirection);
            item = null;
        }
        else if (item && item is Trash)
        {
            ((Trash)item).throwTrash(Vector2.right * FacingDirection);
            item = null;
        }
    }

    void processStashItem()
    {
        //Stash / Swap items
        swapItems();
    }

    public void swapItems()
    {
        //Swap items without dropping anything
        Collectible prevStash = stashedItem;
        stashedItem = item;
        _item = null;
        item = prevStash;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Slam
        if (Crouching)
        {
            BreakableBlock bb = collision.gameObject.GetComponent<BreakableBlock>();
            if (bb)
            {
                bb.breakBlock();
            }
            //push back all movable things in the area
            Collider2D[] coll2Ds = new Collider2D[100];
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, slamRadius, coll2Ds);
            for (int i = 0; i < count; i++)
            {
                Collider2D c2d = coll2Ds[i];
                if (c2d)
                {
                    if (!c2d.isTrigger)
                    {
                        Rigidbody2D rb2dColl = c2d.gameObject.GetComponent<Rigidbody2D>();
                        if (rb2dColl)
                        {
                            rb2dColl.AddForce((coll2d.transform.position - transform.position).normalized * slamForce);
                        }
                        //Defeat enemy
                        Enemy enemy = c2d.gameObject.GetComponentInParent<Enemy>();
                        if (enemy)
                        {
                            enemy.defeatEnemy();
                        }
                    }
                }
            }
            //Uncrouch (if forced to stay crouched mid-air)
            if (!crouchingInput)
            {
                Crouching = false;
            }
            //Update moveSpeed (apply crouch slow)
            if (Crouching && moveSpeed != 0 && Grounded)
            {
                moveSpeed = Mathf.Sign(moveSpeed) * crouchMoveSpeed * speedFactor;
            }
        }
    }

    public void kill()
    {
        if (checkPoint)
        {
            //Teleport player to checkpoint
            transform.position = checkPoint.transform.position;
            //Teleport pan to checkpoint
            if ((!item || item.name != "pan")
                && (!stashedItem || stashedItem.name != "pan"))
            {
                foreach (Weapon weapon in FindObjectsOfType<Weapon>())
                {
                    if (weapon.name == "pan")
                    {
                        weapon.transform.position = checkPoint.transform.position;
                    }
                }
            }
            //Reset enemies
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                enemy.resetEnemy();
            }
        }
        else
        {
            SceneManager.LoadScene(gameObject.scene.name);
        }
    }
}
