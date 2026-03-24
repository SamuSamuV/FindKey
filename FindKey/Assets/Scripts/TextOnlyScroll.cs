using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextOnlyScroll : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public ScrollRect scrollRect;

    public void AddText(string newText)
    {
        tmpText.text += newText + "\n";

        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();

        yield return new WaitForEndOfFrame();

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}