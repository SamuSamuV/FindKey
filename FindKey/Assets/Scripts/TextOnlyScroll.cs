/// <summary>
/// Class: TextOnlyScroll
/// Description: This class is responsible for managing a TextMeshProUGUI component and a ScrollRect to display text in a scrollable area.
///              It provides a method to add new text to the TextMeshProUGUI and automatically scrolls to the bottom of the ScrollRect when new text is added.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextOnlyScroll : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public ScrollRect scrollRect;

    public void AddText(string newText) // Method to add new text to the TextMeshProUGUI and scroll to the bottom
    { 
        tmpText.text += newText + "\n";

        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom() // Coroutine to scroll to the bottom of the ScrollRect after adding new text
    {
        Canvas.ForceUpdateCanvases();

        yield return new WaitForEndOfFrame();

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}