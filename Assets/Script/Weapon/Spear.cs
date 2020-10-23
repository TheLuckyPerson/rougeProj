using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapons
{
    public override void Use()
    {
        player.Spear();
    }

    public override void Utility()
    {
        player.DoubleJump(this);
    }
}
