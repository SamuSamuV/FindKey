using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public MoveAppData moveAppData;

    public Transform inventoryGrid;
    public GameObject inventoryItemPrefab;

    public Sprite axeSprite;

    void Start()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        moveAppData = goMoveAppData.GetComponent<MoveAppData>();

        if (moveAppData.hasAxe)
            AddItemToInventory(axeSprite, "Axe");
    }

    public void AddItemToInventory(Sprite itemSprite, string itemName)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventoryGrid);
        InventoryItem item = newItem.GetComponent<InventoryItem>();
        if (item != null)
            item.SetItem(itemSprite, itemName);
    }

    public void AddAxeToInventary()
    {
        AddItemToInventory(axeSprite, "Axe");
    }
}