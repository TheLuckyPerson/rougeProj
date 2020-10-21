using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public float moveSpeed = 3;
    public Transform player;
    public Vector3 playerDir;

    public LayerMask wallLayer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        VariableUpdate();
        AttackCycle();
    }

    void FixedUpdate()
    {
        MoveCycle();
    }

    void VariableUpdate()
    {
        playerDir = player.transform.position.x >= transform.position.x ? Vector3.right : Vector3.left;
    }

    public virtual void MoveCycle()
    {
        movement = Time.fixedDeltaTime * moveSpeed;
        WallCorrection(playerDir, wallLayer, ref movement);
        transform.position += movement * playerDir;
    }

    public virtual void AttackCycle()
    {

    }
}
