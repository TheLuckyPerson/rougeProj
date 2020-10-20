using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Sword", order = 1)]
public class Sword : Weapons
{
    public override void Use()
    {
        player.Sword();
    }

    public override void Utility()
    {
        player.Dash();
    }
}
