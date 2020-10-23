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
    public float jumpPw = 6f;

    [Space]
    [Header("Detection")]
    public LayerMask groundLayer;
    public bool isGrounded = true;
    private bool isDashing = false;
    private bool isRecoil = false;

    [Space]
    [Header("Weapon")]
    public GameObject weaponObject;
    public GameObject weaponObject2;
    public Transform weaponHolder;
    private Weapons weaponScript;
    private Weapons weaponScript2;
    private Weapons weaponDashScript;

    private Animator anim;

    Vector3 dir;
    private bool isUsingWeapon;
    Vector3 facing = Vector3.right;
    Vector3 lastValidFacing;

    float startDashX;
    float dashDelta;

    private GameObject weaponG;
    private GameObject weaponG2;

    private Vector3 recoilDi;
    private float recoilAmt;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SetWeapon(weaponObject, ref weaponScript, ref weaponG);
        SetWeapon(weaponObject2, ref weaponScript2, ref weaponG2);
    }

    void SetWeapon(GameObject weap, ref Weapons weapScript, ref GameObject weapObj)
    {
        if(weap) {
            if(weapObj) {
                Destroy(weapObj);
            } 

            weapObj = Instantiate(weap, weaponHolder.transform.position, Quaternion.identity, weaponHolder);
                weapScript = weapObj.GetComponent<Weapons>();
                weapScript.player = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        VaribleUpdates();
        Jump();
        VerticalMovementCorrection(groundLayer);
        UseWeapon();
        UseUtility();
        DevOnly();
        Movement();
    }
    public bool devMode = false;
    private void DevOnly()
    {
        if(devMode) {
            if(Input.GetKeyDown("f")) {
                SetWeapon(weaponObject, ref weaponScript, ref weaponG);
            }

            if(Input.GetKeyDown("r")) {
                UTIL.Reload();
            }
        }
    }

    private void FixedUpdate()
    {
        // Movement();
    }

    void VaribleUpdates()
    {
        isGrounded = CollisionDetect(Vector3.down, .02f, groundLayer) != -1;
        dir = Vector3.right * Input.GetAxis("Horizontal");
        isUsingWeapon =  weaponHolder.gameObject.activeSelf;
        if(isGrounded) {
            ResetWeaponGrounded();
            if(rb2d.velocity.y < 0) rb2d.velocity = Vector3.zero; // attempt to get rid of going through floor
        }
        if (dir != Vector3.zero) {
            facing = dir;
            if(!isUsingWeapon)
                transform.localScale = new Vector3(facing.x, 1, 1);
        }
    }

    void ResetWeaponGrounded()
    {
        if(weaponScript) {
            weaponScript.dashUtilityAir = false;
            weaponScript.canDoubleJump = true;
        }
        if(weaponScript2) {
            weaponScript2.dashUtilityAir = false;
            weaponScript2.canDoubleJump = true;
        }
    }

    void Movement()
    {
        if (!isDashing) { // is able to control movement
            movement = Time.deltaTime * speed;
            WallCorrection(dir, groundLayer, ref movement);
            transform.position += movement * dir;
            lastValidFacing = facing;
        } else { // dash movement
            movement = Time.deltaTime * weaponDashScript.dashSpeed;
            float trueMovement = movement; // not affected by walls
            if(dashDelta + trueMovement > weaponDashScript.dashLength) {// will go past dash distance
                StopDash();
                movement = dashDelta + trueMovement - dashDelta;
            } else {
                dashDelta += trueMovement; // true distance moved since dashing
            }
            WallCorrection(lastValidFacing, groundLayer, ref movement);
            transform.position += movement * lastValidFacing;
        }

        if(isRecoil) {
            float move = recoilAmt;
            WallCorrection(recoilDi, groundLayer, ref move);
            transform.position += move * recoilDi;
            isRecoil = false;
        }
    }

    void UseWeapon()
    {
        if(!isUsingWeapon) {
            if (weaponScript && Input.GetButton("Attack")) {
                weaponScript.Use();
                ToggleWeapons(weaponScript, weaponScript2);
            }
            else if (weaponScript2 && Input.GetButton("Attack2")) {
                weaponScript2.Use();
                ToggleWeapons(weaponScript2, weaponScript);
            }
        }
    }

    void UseUtility()
    {
        if (weaponScript && Input.GetButtonDown("Utility")) {
            weaponScript.Utility();
        } else if (weaponScript2 && Input.GetButtonDown("Utility2")) {
            weaponScript2.Utility();
        }
    }

    void ToggleWeapons(Weapons w1, Weapons w2)
    {
        w1.gameObject.SetActive(true);
        if(w2) w2.gameObject.SetActive(false);
    }

    void Jump()
    {
        if (Input.GetButton("Jump")) {
            if(isGrounded) {
                ApplyJump(jumpPw);
            }
        }
    }

    private void ApplyJump(float pwr)
    {
        rb2d.velocity = Vector2.up * pwr;
        if(isDashing) {
            StopDash();
        }
    }

    public void Dash(Weapons weap)
    {
        if(!isDashing) {
            if(!isGrounded && weap.dashUtilityAir) {
                return;
            } else if(!isGrounded) {
                weap.dashUtilityAir = true;
            }
            rb2d.velocity = Vector3.zero;
            rb2d.gravityScale = 0;
            startDashX = transform.position.x;
            weaponDashScript = weap;
            isDashing = true;
        }
    }
    public void DoubleJump(Weapons weap)
    {
        if(!isGrounded && weap.canDoubleJump) {
            ApplyJump(weap.doubleJumpPwr);
            weap.canDoubleJump = false;
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
        weaponHolder.parent.rotation = Quaternion.identity;
        anim.SetTrigger("swing_sword");
    }

    public void Recoil(float amt, Vector3 di)
    {
        if(dir == Vector3.zero && !isDashing) {
            recoilAmt = amt;
            recoilDi = di;
            isRecoil = true;
        }
    }

    public void Spear()            
    {
        Vector2 positionDifference;
        float angle;
        Vector2 mposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // calc
        positionDifference = (mposition - (Vector2)transform.position).normalized;
        angle = Mathf.Atan2(positionDifference.y, positionDifference.x);
        angle = angle * Mathf.Rad2Deg + (transform.localScale.x == 1 ? 0 : 180);

        weaponHolder.parent.rotation = Quaternion.Euler(0, 0, angle);

        // Recoil(.2f, Vector3.right * (positionDifference.x < 0 ? -1 : 1));
        anim.SetTrigger("stab_spear");
    }
}
