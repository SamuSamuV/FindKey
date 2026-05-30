/// <summary>
/// Class: InventoryManager
/// Description: This script manages the inventory system in the FindKey game. It is responsible for displaying the items in the player's inventory and allowing for dynamic updates when
///              items are added or removed. The script references a Transform for the inventory grid where item icons will be displayed and a prefab for the inventory item UI element.
///              When the inventory is refreshed, it clears the current items and populates the grid with the saved items from the MoveAppData component, which holds the player's inventory data.
///              Each item is instantiated as a new UI element in the grid, displaying its sprite and name.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Transform inventoryGrid;
    public GameObject inventoryItemPrefab;

    void Start() // Refresh the inventory at the start of the game to display any saved items
    {
        RefreshInventory();
    }

    public void RefreshInventory() // Clears the current inventory display and repopulates it with the saved items from MoveAppData
    {
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
        {
            MoveAppData moveAppData = goMoveAppData.GetComponent<MoveAppData>();
            if (moveAppData != null)
            {
                foreach (var savedItem in moveAppData.savedItems) // Loop through the saved items and add them to the inventory display
                {
                    AddItemToInventory(savedItem.itemSprite, savedItem.itemName);
                }
            }
        }
    }

    public void AddItemToInventory(Sprite itemSprite, string itemName) // Instantiates a new inventory item UI element in the grid and sets its sprite and name
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventoryGrid);
        InventoryItem item = newItem.GetComponent<InventoryItem>();
        if (item != null)
            item.SetItem(itemSprite, itemName);
    }
}