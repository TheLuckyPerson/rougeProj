using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EventSystem.current.IsPointerOverGameObject() to fix craft and swing
public class Player : Entity
{
    [Header("Player Stats")]
    public float health = 3;

    [Space]
    [Header("Movement Variables")]
    public float speed = 7f;
    public float dashSpeed = 24f;
    public float dashDist = 1f;
    public float jumpPw = 6f;

    [Space]
    [Header("Detection")]
    public LayerMask groundLayer;
    public bool isGrounded = true;
    private bool isDashing = false;
    private bool usedUngroundedDash = false;

    [Space]
    [Header("Weapon")]
    public GameObject weaponObject;
    public Transform weaponHolder;
    private Weapons weapon;

    private Rigidbody2D rb2d;
    private Animator anim;
    private Transform weaponTransform;

    Vector3 dir;
    private bool isUsingWeapon;
    Vector3 facing = Vector3.right;
    Vector3 lastValidFacing;

    float startDashX;
    float dashDelta;

    string weaponStr;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        GameObject g = Instantiate(weaponObject, weaponHolder.transform.position, Quaternion.identity, weaponHolder);
        weapon = g.GetComponent<Weapons>();
        weapon.player = this;
        weaponTransform = g.transform;
    }

    // Update is called once per frame
    void Update()
    {
        VaribleUpdates();
        Jump();
        VerticalMovementCorrection();
        UseWeapon();
        UseUtility();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void VaribleUpdates()
    {
        isGrounded = CollisionDetect(Vector3.down, .02f, groundLayer) != -1;
        dir = Vector3.right * Input.GetAxis("Horizontal");
        isUsingWeapon =  weaponHolder.gameObject.activeSelf;
        if(isGrounded) {
            usedUngroundedDash = false;
            if(rb2d.velocity.y < 0) rb2d.velocity = Vector3.zero; // attempt to get rid of going through floor
        }
        if (dir != Vector3.zero) {
            facing = dir;
            if(!isUsingWeapon)
                transform.localScale = new Vector3(facing.x, 1, 1);
        }
    }

    void VerticalMovementCorrection()
    {
        if(rb2d.velocity.y!= 0) {
            float currentVertVel = rb2d.velocity.y*Time.deltaTime;
            Vector3 v = rb2d.velocity.normalized;
            if(WallCorrection(v, groundLayer, ref currentVertVel)) { // next vert movement will go past a ground object
                transform.position += v * currentVertVel;
                rb2d.velocity = Vector3.zero;
            }
        }
    }
    void Movement()
    {
        if (!isDashing) { // is able to control movement
            movement = Time.fixedDeltaTime * speed;
            WallCorrection(dir, groundLayer, ref movement);
            transform.position += movement * dir;
            lastValidFacing = facing;
        } else { // dash movement
            movement = Time.fixedDeltaTime * dashSpeed;
            float trueMovement = movement; // not affected by walls
            if(dashDelta + trueMovement > dashDist) {// will go past dash distance
                StopDash();
                movement = dashDelta + trueMovement - dashDelta;
            } else {
                dashDelta += trueMovement; // true distance moved since dashing
            }
            WallCorrection(lastValidFacing, groundLayer, ref movement);
            transform.position += movement * lastValidFacing;
        }
    }

    void UseWeapon()
    {
        if (Input.GetButton("Attack") && !isUsingWeapon) {
            weapon.Use();
        }
    }

    void UseUtility()
    {
        if (Input.GetButtonDown("Utility")) {
            weapon.Utility();
        }
    }

    void Jump()
    {
        if (Input.GetButton("Jump") && isGrounded) {
            rb2d.velocity = Vector2.up * jumpPw;
            if(isDashing) {
                StopDash();
            }
        }
    }

    public void Dash()
    {
        if(!isDashing) {
            if(!isGrounded && usedUngroundedDash) {
                return;
            } else if(!isGrounded) {
                usedUngroundedDash = true;
            }
            rb2d.velocity = Vector3.zero;
            rb2d.gravityScale = 0;
            startDashX = transform.position.x;
            isDashing = true;
        }
    }

    /* 
    resets dash changes
     */
    private void StopDash()
    {
        isDashing = false;
        rb2d.gravityScale = 1;
        dashDelta = 0;
    }

    public void Sword()
    {
        anim.SetTrigger("swing_sword");
    }

    public void Spear()            
    {
        Vector2 positionDifference;
        float angle;
        Vector2 mposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionDifference = (mposition - (Vector2)transform.position).normalized;
        angle = Mathf.Atan2(positionDifference.y, positionDifference.x);
        angle = angle * Mathf.Rad2Deg + (transform.localScale.x == 1 ? 0 : 180);
        weaponHolder.parent.rotation = Quaternion.Euler(0, 0, angle);
        anim.SetTrigger("stab_spear");
    }
}
