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

    public void AddLineAnimated(string text, System.Action onFinished = null)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        StartCoroutine(TypewriterRoutine(text, onFinished));
    }

    // Añadimos "= null" para que si no le pasamos nada, no se queje
    IEnumerator TypewriterRoutine(string lineToAdd, System.Action onFinished = null)
    {
        if (storyText.text.Length > 0) storyText.text += "\n";

        bool isInsideTag = false;

        for (int i = 0; i < lineToAdd.Length; i++)
        {
            char c = lineToAdd[i];
            if (c == '<') isInsideTag = true;
            storyText.text += c;
            if (c == '>') isInsideTag = false;

            if (!isInsideTag)
            {
                UpdateLayout();
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        typingCoroutine = null;

        // ¡AQUÍ ESTÁ LA CLAVE!
        // Si nos pasaron una misión final (como "activar input"), la ejecutamos ahora.
        onFinished?.Invoke();
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