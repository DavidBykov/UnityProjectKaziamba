using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionUI : MonoBehaviour
{
    public Text itemNameText;
    public Image itemImage;
    public Text itemDescriptionText;

    private ItemConfiguration _itemConfiguration;

    private void OnEnable()
    {
        itemNameText.text = _itemConfiguration.itemName;
        itemImage.sprite = _itemConfiguration.itemImage;
        itemDescriptionText.text = _itemConfiguration.itemDescription;
    }

    public void SetItemConfiguration(ItemConfiguration itemConfiguration)
    {
        _itemConfiguration = itemConfiguration;
    }

}
