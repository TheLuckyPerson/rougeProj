using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Player player;

    [Space]
    [Header("Stats")]
    public float damage = 2;
    public Vector3 kbRate;

    [Space]
    [Header("Dash(Optional)")]
    public bool dashUtilityAir = false; // for any weapon using dashUtility
    public float dashSpeed = 25f;
    public float dashLength = 3;

    [Space]
    [Header("Double Jump(Optional)")]
    public bool canDoubleJump = true;
    public float doubleJumpPwr = 7f;

    // Start is called before the first frame update

    public virtual void Use()
    {
        
    }

    public virtual void Utility()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == ("Enemy")) {
            Vector3 dirfixedKb = new Vector3((player.transform.position.x < col.transform.position.x ? 1 : -1) * kbRate.x, kbRate.y);
            print(dirfixedKb);
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage, dirfixedKb);
        }
    }
}
