using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryLog : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text storyText; // Asegúrate de que esto es TextMeshProUGUI en el Inspector
    public RectTransform contentRect;
    public ScrollRect scrollView;

    [Header("Configuración")]
    public float typingSpeed = 0.03f; // Segundos por letra (más bajo = más rápido)

    private Coroutine typingCoroutine;

    [HideInInspector]
    public string lastLoadedText = "";
    // Método original (instantáneo)
    public void AddLine(string text)
    {
        // Si estábamos escribiendo algo, lo terminamos de golpe antes de poner lo nuevo
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (storyText.text.Length > 0) storyText.text += "\n";
        storyText.text += text;

        UpdateLayout();
    }

    public void SetText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        storyText.text = text;
        UpdateLayout();
    }

    public void AddLineAnimated(string text)
    {
        // Si ya hay una animación, la paramos (o podrías encolarla si prefieres)
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        StartCoroutine(TypewriterRoutine(text));
    }

    IEnumerator TypewriterRoutine(string lineToAdd)
    {
        // 1. Añadimos el salto de línea si no es el primer mensaje
        if (storyText.text.Length > 0) storyText.text += "\n";

        // 2. Guardamos la longitud actual para saber dónde empezar a añadir letras
        int startIndex = storyText.text.Length;

        // 3. Bucle letra a letra
        // Analizamos si es una etiqueta HTML (ej: <color=red>) para escribirla de golpe
        bool isInsideTag = false;

        for (int i = 0; i < lineToAdd.Length; i++)
        {
            char c = lineToAdd[i];

            // Detección de etiquetas Rich Text
            if (c == '<') isInsideTag = true;

            // Añadimos el caracter al texto final
            storyText.text += c;

            // Si es el cierre de etiqueta, dejamos de estar "dentro"
            if (c == '>') isInsideTag = false;

            // SOLO esperamos si NO estamos dentro de una etiqueta.
            // Así las etiquetas se escriben instantáneamente y no se rompe el formato visual.
            if (!isInsideTag)
            {
                // Forzamos al Canvas a actualizarse para que el Scroll baje con el texto
                UpdateLayout();
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        typingCoroutine = null;
    }

    void UpdateLayout()
    {
        // Forzamos actualización de layout para que el scroll funcione
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        // Hacemos scroll al fondo
        if (scrollView != null) scrollView.verticalNormalizedPosition = 0f;
    }

    public void SetTextAnimated(string text)
    {
        if (!text.Contains("You can't do that here."))
        {
            lastLoadedText = text;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        storyText.text = "";
        typingCoroutine = StartCoroutine(TypewriterRoutine(text));
    }
}