using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public List<Transform> colliders;
    public float movement;
    private const float COLLISION_CONTACT_OFFSET = .02f; 

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
     */
    public void WallCorrection(Vector3 dir_, LayerMask layer)
    {
        float colPredict = CollisionDetect(dir_, movement, layer);
        if (colPredict != -1) { // next move goes past a wall
            if(colPredict > COLLISION_CONTACT_OFFSET) // keep in mind unity collision offset
                movement = colPredict - COLLISION_CONTACT_OFFSET;
            else
                movement = 0;
        }
    }
}
