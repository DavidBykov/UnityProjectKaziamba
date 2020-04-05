using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemsMenuUI : MonoBehaviour
{
    public GameObject buttonsPanelPrefab;
    public GameObject buttonPrefab;
    public ItemDescriptionUI itemDescriptionUI;

    public Transform contentTransform;
    public List<ItemConfiguration> itemConfigurations = new List<ItemConfiguration>();

    private Dictionary<Button, ItemConfiguration> buttonLevelPairs = new Dictionary<Button, ItemConfiguration>();

    public void OnEnable()
    {
        for (int i = 0; i < contentTransform.childCount; i++)
            Destroy(contentTransform.GetChild(i).gameObject);

        itemConfigurations = Resources.LoadAll("Items", typeof(ItemConfiguration)).Cast<ItemConfiguration>().ToList();
        buttonLevelPairs.Clear();

        GameObject newPanel = Instantiate(buttonsPanelPrefab, contentTransform);
        for (int i = 0; i < itemConfigurations.Count; i++)
        {
            if (i % 3 == 0 && i != 0)
            {
                newPanel = Instantiate(buttonsPanelPrefab, contentTransform);
            }

            GameObject newButton = Instantiate(buttonPrefab, newPanel.transform);
            Button newUIButton = newButton.GetComponent<Button>();
            newUIButton.GetComponentsInChildren<Image>().ToList().Last().sprite = itemConfigurations[i].itemImage;
            newUIButton.onClick.AddListener(() => ButtonClicked(newUIButton));
            buttonLevelPairs.Add(newUIButton, itemConfigurations[i]);
        }
    }

    private void ButtonClicked(Button clickedButton)
    {
        ItemConfiguration item;
        buttonLevelPairs.TryGetValue(clickedButton, out item);
        if (item)
        {
            itemDescriptionUI.SetItemConfiguration(item);
            itemDescriptionUI.gameObject.SetActive(true);
        }
    }
}
