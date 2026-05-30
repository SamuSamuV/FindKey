using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Class: SavedInventoryItem
/// Description: This class represents an item in the player's inventory for the FindKey game. It contains two public fields: itemName, which is a string representing
/// the name of the item, and itemSprite, which is a Sprite representing the visual representation of the item in the inventory. This class is marked as [System.Serializable]
/// to allow instances of it to be serialized and displayed in the Unity Inspector, making it easier for developers to manage and visualize the inventory items within the editor.
/// </summary>
[System.Serializable]
public class SavedInventoryItem
{
    public string itemName;
    public Sprite itemSprite;
}

/// <summary>
/// File: MoveAppData
/// Description: This script serves as a data container for the movement map in the FindKey game. It holds various boolean variables that track the state of the game
///              related to the player's interactions and progress in the movement map, such as whether the player has seen certain events, has specific items, or is
///              in certain positions. Additionally, it contains a list of saved inventory items to maintain a visual memory of the player's inventory state. This data
///              can be accessed and modified by other scripts to manage the game's narrative and mechanics based on the player's actions in the movement map.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
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
    public List<SavedInventoryItem> savedItems = new List<SavedInventoryItem>(); // List to store the saved inventory items for visual memory
}