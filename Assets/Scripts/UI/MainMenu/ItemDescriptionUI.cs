using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionUI : MonoBehaviour
{
    public Text itemNameText;
    public Image itemImage;
    public Text itemDescriptionText;
    public GameObject soldButton;
    public GameObject soldLabel;
    public GameObject moneyInfo;
    public Text ItemCost;
    public Text playerMoney;

    private ItemConfiguration _itemConfiguration;

    private void OnEnable()
    {
        itemNameText.text = _itemConfiguration.itemName;
        itemImage.sprite = _itemConfiguration.itemImage;
        itemDescriptionText.text = _itemConfiguration.itemDescription;
        ItemCost.text = _itemConfiguration.itemCost.ToString();
        playerMoney.text = GameEconomy.GetPlayerMoney().ToString();
        if (_itemConfiguration.bought)
        {
            moneyInfo.SetActive(false);
            soldButton.SetActive(false);
            soldLabel.SetActive(true);
        } else
        {
            moneyInfo.SetActive(true);
            soldButton.SetActive(true);
            soldLabel.SetActive(false);
        }
    }

    public void SetItemConfiguration(ItemConfiguration itemConfiguration)
    {
        _itemConfiguration = itemConfiguration;
    }

    public void TryBuyItem()
    {
        if(GameEconomy.GetPlayerMoney() >= _itemConfiguration.itemCost)
        {
            GameEconomy.SpendPlayerMoney(_itemConfiguration.itemCost);
            _itemConfiguration.bought = true;
            soldButton.SetActive(false);
            soldLabel.SetActive(true);
            moneyInfo.SetActive(false);
            playerMoney.text = GameEconomy.GetPlayerMoney().ToString();
            GameEconomy.curentItem = _itemConfiguration;
        } else
        {

        }
    }
}
