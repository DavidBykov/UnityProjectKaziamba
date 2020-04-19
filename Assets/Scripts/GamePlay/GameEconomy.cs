using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEconomy
{
    private static int playerMoney;
    public static ItemConfiguration curentItem;
    public static LevelConfiguration curentLevel;

    public static void SaveEconomy()
    {
        PlayerPrefs.SetInt("PLAYER_MONEY", playerMoney);
    }

    public static void LoadEcomomy()
    {
        playerMoney = PlayerPrefs.GetInt("PLAYER_MONEY");
    }

    public static void DropEconomy()
    {
        playerMoney = 0;
        curentItem = null;
        SaveEconomy();
    }

    public static int GetPlayerMoney()
    {
        LoadEcomomy();
        return playerMoney;
    }

    public static void SpendPlayerMoney(int money)
    {
        playerMoney -= money;
        SaveEconomy();
        LoadEcomomy();
    }

    public static void AddPlayerMoney(int money)
    {
        playerMoney += money;
        SaveEconomy();        
    }
}
