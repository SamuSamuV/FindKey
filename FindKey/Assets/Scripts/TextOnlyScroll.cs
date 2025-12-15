using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextOnlyScroll : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public ScrollRect scrollRect;

    public void AddText(string newText)
    {
        tmpText.text += newText + "\n";

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}