/// <summary>
/// Class: MoveAppData
/// Description: This script serves as a data container for the movement map in the FindKey game. It holds various boolean variables that track the state of the game
///              related to the player's interactions and progress in the movement map, such as whether the player has seen certain events, has specific items, or is
///              in certain positions. Additionally, it contains a list of saved inventory items to maintain a visual memory of the player's inventory state. This data
///              can be accessed and modified by other scripts to manage the game's narrative and mechanics based on the player's actions in the movement map.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

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