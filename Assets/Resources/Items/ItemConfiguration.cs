using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameParameter
{
    GameTime,
    StartSoulsCount,
    AccelerationDuration,
    SlowdownDuration,
    SoulSpeed,
    SoulWalkingSpeed,
    SoulDetectionDistance,
    PlayerDetectionDistance,
    PlayerSpeed,
    PlayerSpeedWhenTargetingSouls
}

[System.Serializable]
public struct ChangingParameter
{
    public GameParameter gameParameter;
    public float value;
    public bool useAsPercent;
}

[CreateAssetMenu(fileName = "Item ", menuName = "Item configuration", order = 51)]
public class ItemConfiguration : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public Sprite itemBoughtImage;
    public int itemCost;
    public bool bought;

    public List<ChangingParameter> changingParameters = new List<ChangingParameter>();
}
