using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetText(string text)
    {
        storyText.text = text;
    }

    //Esta linea simplemente me lka guardo para mostrar en negrita el texto a la derecha en negrita lo que escribe el jugador
    //storyLog.SetText($"<align=right><b>{input}</b></align>");
}