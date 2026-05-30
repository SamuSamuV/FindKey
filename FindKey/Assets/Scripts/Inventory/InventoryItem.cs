/// <summary>
/// Class: InventoryItem
/// Description: This script represents an individual item in the inventory system of the FindKey game. It manages the display of the item's image and name within the inventory UI.
///              The script contains references to an Image component for showing the item's sprite and a TextMeshPro text component for displaying the item's name. The SetItem method allows for
///              setting both the sprite and the name of the item, which can be called when adding new items to the inventory or updating existing ones. This class is designed to work in conjunction
///              with an InventoryManager that handles the overall inventory system and item management.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemNameText;

    public void SetItem(Sprite sprite, string name) // This is to set the item sprite and name, called by InventoryManager when adding items to the inventory
    {
        if (itemImage != null)
            itemImage.sprite = sprite;
        if (itemNameText != null)
            itemNameText.text = name;
    }
}