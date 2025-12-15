using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemNameText;

    public void SetItem(Sprite sprite, string name)
    {
        if (itemImage != null)
            itemImage.sprite = sprite;
        if (itemNameText != null)
            itemNameText.text = name;
    }
}