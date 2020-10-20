using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Player Stats")]
    public float health = 3;

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

    [Space]
    [Header("Weapon")]
    public Weapons weapon;
    public SpriteRenderer weaponSprite;

    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer spr;
    private Transform weaponTransform;

    Vector3 dir;
    private bool isUsingWeapon;
    Vector3 facing = Vector3.right;
    Vector3 lastValidFacing;

    float startDashX;
    float dashDelta;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        weapon.player = this;
        weaponSprite.sprite = weapon.weaponSprite;
        weaponTransform = weaponSprite.transform;
    }

    // Update is called once per frame
    void Update()
    {
        VaribleUpdates();
        Jump();
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
        isUsingWeapon =  weaponTransform.gameObject.activeSelf;
        if (dir != Vector3.zero) {
            facing = dir;
            if(!isUsingWeapon)
                transform.localScale = new Vector3(facing.x, 1, 1);
        }
    }

    void Movement()
    {
        if (!isDashing) { // is able to control movement
            movement = Time.fixedDeltaTime * speed;
            WallCorrection(dir, groundLayer);
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
            WallCorrection(lastValidFacing, groundLayer);
            transform.position += movement * lastValidFacing;
        }
    }

    void UseWeapon()
    {
        if (Input.GetButtonDown("Attack") && !isUsingWeapon) {
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
        if (Input.GetButtonDown("Jump") && isGrounded) {
            rb2d.velocity = Vector2.up * jumpPw;
            if(isDashing) {
                StopDash();
            }
        }
    }

    public void Dash()
    {
        if(!isDashing) {
            rb2d.velocity = Vector3.zero;
            rb2d.gravityScale = 0;
            startDashX = transform.position.x;
            isDashing = true;
        }
    }

    public void Sword()
    {
        anim.SetTrigger("swing_sword");
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
}
