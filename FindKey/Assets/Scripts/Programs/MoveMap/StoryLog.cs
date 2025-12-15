using TMPro;
using UnityEngine;
using UnityEngine.UI; // Necesario para ScrollRect y Canvas

public class StoryLog : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text storyText;
    public RectTransform contentRect;
    public ScrollRect scrollView;

    [Header("Configuración")]
    public float paddingBottom = 20f;

    public void AddLine(string line)
    {
        storyText.text += "\n" + line;
        storyText.ForceMeshUpdate();

        float preferredHeight = storyText.preferredHeight;

        Vector2 size = contentRect.sizeDelta;
        size.y = preferredHeight + paddingBottom;
        contentRect.sizeDelta = size;
        Canvas.ForceUpdateCanvases();

        scrollView.verticalNormalizedPosition = 0f;
    }
}