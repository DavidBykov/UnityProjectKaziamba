using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    PlayerSpeedWhenTargetingSouls,
    PlayerLifesCount
}

[System.Serializable]
public class ChangingParameter
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
    public bool cantBuy;
    public int itemCost;
    public bool bought;

    public List<ChangingParameter> changingParameters = new List<ChangingParameter>();

    public float TryModifyParameter(GameParameter gameParameter, float value)
    {
        ChangingParameter changingParameter = null;

        try
        {
            changingParameter = changingParameters.Single(s => s.gameParameter == gameParameter);
        }
        catch (System.InvalidOperationException)
        {

        }

        if (changingParameter != null)
        {
            Debug.Log("Нашел параметр в предмете " + changingParameter.gameParameter);
            if (changingParameter.useAsPercent)
            {
                return value *= changingParameter.value;
            }
            else
            {
                return value += changingParameter.value;
            }
        }
        else
        {
            return value;
        }
    }

    public ChangingParameter GetParameterByEnum(GameParameter gameParameter)
    {
        ChangingParameter changingParameter = null;

        try
        {
            changingParameter = changingParameters.Single(s => s.gameParameter == gameParameter);
        }
        catch (System.InvalidOperationException)
        {

        }

        return changingParameter;
    }
}
