using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public List<Transform> colliders;
    public float movement;
    private const float COLLISION_CONTACT_OFFSET = .02f; 
    public Rigidbody2D rb2d;

    /* 
        raycast from all 4 edge colliders in @param dir for @param dist 
        @return min dist of collision or -1 if not found 
     */
    public float CollisionDetect(Vector3 dir, float dist, LayerMask layer)
    {
        float minDist = dist;
        foreach (Transform t in colliders) { // loop through all colliders
            RaycastHit2D r = Physics2D.Raycast(t.position, dir, dist, layer);
            if (r && r.distance < minDist) { // a collider detects a valid prediction
                minDist = r.distance;
            }
        }
        return minDist == dist ? -1 : minDist;
    }

    /* 
    Corrects for movemnt against a wall in @pram dir_
    @return true if corrected
     */
    public bool WallCorrection(Vector3 dir_, LayerMask layer, ref float move)
    {
        float colPredict = CollisionDetect(dir_, movement, layer);
        if (colPredict != -1) { // next move goes past a wall
            if(colPredict > COLLISION_CONTACT_OFFSET) // keep in mind unity collision offset
                move = colPredict - COLLISION_CONTACT_OFFSET;
            else
                move = 0;
            return true;
        }
        return false;
    }

    public void Recoil(float amt, Vector3 di, LayerMask groundLayer)
    {
        float move = amt;
        WallCorrection(di, groundLayer, ref move);
        transform.position += move * di;
    }

    public void ApplyKb(Vector3 kb, LayerMask l)
    {
        rb2d.velocity = Vector3.up * kb.y;
        Vector3 di = kb.x > 0 ? Vector3.right : Vector3.left;
        Recoil(Mathf.Abs(kb.x), di, l);
    }

    public void VerticalMovementCorrection(LayerMask l)
    {
        if(rb2d.velocity.y!= 0) {
            float currentVertVel = rb2d.velocity.y*Time.deltaTime;
            Vector3 v = rb2d.velocity.normalized;
            if(WallCorrection(v, l, ref currentVertVel)) { // next vert movement will go past a ground object
                transform.position += v * currentVertVel;
                rb2d.velocity = Vector3.zero;
            }
        }
    }
}
