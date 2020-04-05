using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item ", menuName = "Item configuration", order = 51)]
public class ItemConfiguration : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public float itemCost;
    public bool bought;
}
