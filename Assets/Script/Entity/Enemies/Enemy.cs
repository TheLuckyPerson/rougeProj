using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public float moveSpeed = 3;

    public float health = 5;
    public bool inStun = false;
    public float stunTime;
    public Transform player;
    public Vector3 playerDir;

    public LayerMask wallLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        VariableUpdate();
        MoveCycle();
        VerticalMovementCorrection(wallLayer);
        AttackCycle();
    }

    void FixedUpdate()
    {
        // MoveCycle();
    }

    void VariableUpdate()
    {
        playerDir = player.transform.position.x >= transform.position.x ? Vector3.right : Vector3.left;
    }

    private float stunTimer;
    public virtual void MoveCycle()
    {
        if(!inStun) {
            movement = Time.deltaTime * moveSpeed;
            WallCorrection(playerDir, wallLayer, ref movement);
            transform.position += movement * playerDir;
        } else {
            stunTimer += Time.deltaTime;
            if(stunTimer >= stunTime) {
                inStun = false;
                stunTimer = 0;
            }
        }
    }

    public virtual void AttackCycle()
    {

    }

    public void TakeDamage(float amt, Vector3 kb)
    {
        health -= amt;
        ApplyKb(kb, wallLayer);
        inStun = true;
        if(health <= 0) Death();
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
