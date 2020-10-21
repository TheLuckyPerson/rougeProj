using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
