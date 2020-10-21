using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Weapon = 0,
    Upgrade = 1,
}

[CreateAssetMenu(fileName = "Crafting", menuName = "ScriptableObjects/Recipe", order = 1)]

public class CraftingRecipe : ScriptableObject
{
    public Material_Handler requirements;

    public GameObject weapon;

    public ItemType itemType;
}