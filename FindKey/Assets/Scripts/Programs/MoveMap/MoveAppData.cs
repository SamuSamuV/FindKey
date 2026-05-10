using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SavedInventoryItem
{
    public string itemName;
    public Sprite itemSprite;
}

public class MoveAppData : MonoBehaviour
{
    public bool playerHasAlreadySeeThis;
    public bool hasAxe;
    public bool hasChest;
    public bool isChestRepaired = false;
    public bool playerIsFrontCat;
    public bool catIsDead;
    public bool playerIsChattingWithCat;

    [Header("Memoria Visual del Inventario")]
    public List<SavedInventoryItem> savedItems = new List<SavedInventoryItem>();
}