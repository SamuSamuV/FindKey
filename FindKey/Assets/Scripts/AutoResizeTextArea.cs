using UnityEngine;
using TMPro;

public class AutoResizeTextArea : MonoBehaviour
{
    public TextMeshProUGUI textElement;
    public RectTransform targetRect;

    void Update()
    {
        // Calcula la altura real que necesita el texto
        float preferredHeight = textElement.preferredHeight;

        // Ajusta el tamaño del panel
        Vector2 size = targetRect.sizeDelta;
        size.y = preferredHeight;
        targetRect.sizeDelta = size;
    }
}