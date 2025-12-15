using UnityEngine;
using TMPro;

public class AutoResizeTextArea : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI textElement;
    public RectTransform contentRect;

    [Header("Configuración")]
    public float paddingBottom = 20f;

    public void ActualizarTexto(string nuevoTexto)
    {
        textElement.text += nuevoTexto + "\n";

        textElement.ForceMeshUpdate();

        float preferredHeight = textElement.preferredHeight;

        Vector2 size = contentRect.sizeDelta;
        size.y = preferredHeight + paddingBottom;
        contentRect.sizeDelta = size;
    }
}