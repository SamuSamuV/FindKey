/// <summary>
/// Class: AutoResizeTextArea
/// Description: This script manages the auto-resizing of a TextMeshProUGUI text area in the FindKey game. It allows for dynamically updating the text content and adjusting
///              the size of the text area to fit the new content. The script references a TextMeshProUGUI component for displaying the text and a RectTransform for adjusting
///              the size of the content area. When new text is added, it forces an update of the mesh to calculate the preferred height of the text and then adjusts the size
///              of the content area accordingly, ensuring that all text is visible without overflow.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using TMPro;

public class AutoResizeTextArea : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI textElement;
    public RectTransform contentRect;

    [Header("Configuración")]
    public float paddingBottom = 20f;

    public void ActualizarTexto(string nuevoTexto) // Funtion to update the text and resize the content area accordingly
    {
        textElement.text += nuevoTexto + "\n";

        textElement.ForceMeshUpdate();

        float preferredHeight = textElement.preferredHeight;

        Vector2 size = contentRect.sizeDelta;
        size.y = preferredHeight + paddingBottom;
        contentRect.sizeDelta = size;
    }
}