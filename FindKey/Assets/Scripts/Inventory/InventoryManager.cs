using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Transform inventoryGrid;
    public GameObject inventoryItemPrefab;

    void Start()
    {
        RefreshInventory();
    }

    // Esta función borra lo viejo y dibuja lo que haya en MoveAppData
    public void RefreshInventory()
    {
        // 1. Limpiamos la cuadrícula (por si había iconos viejos)
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        // 2. Buscamos la memoria global
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
        {
            MoveAppData moveAppData = goMoveAppData.GetComponent<MoveAppData>();
            if (moveAppData != null)
            {
                // 3. Dibujamos todos los objetos guardados
                foreach (var savedItem in moveAppData.savedItems)
                {
                    AddItemToInventory(savedItem.itemSprite, savedItem.itemName);
                }
            }
        }
    }

    public void AddItemToInventory(Sprite itemSprite, string itemName)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventoryGrid);
        InventoryItem item = newItem.GetComponent<InventoryItem>();
        if (item != null)
            item.SetItem(itemSprite, itemName);
    }
}