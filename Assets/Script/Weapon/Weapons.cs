using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Base", order = 1)]
public class Weapons : ScriptableObject
{
    public Player player;
    
    public Sprite weaponSprite;
    // Start is called before the first frame update
    public Weapons()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Use()
    {

    }

    public virtual void Utility()
    {

    }
}
