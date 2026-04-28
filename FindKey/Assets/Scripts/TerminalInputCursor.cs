using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_InputField))]
public class TerminalInputCursor : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El componente InputField real")]
    public TMP_InputField inputField;

    [Tooltip("El TextMeshProUGUI que simulará ser el input visible")]
    public TextMeshProUGUI displayText;

    [Header("Configuración")]
    public string placeholderMessage = "Escribe aqui";
    public float blinkSpeed = 0.5f;
    public string cursorChar = "_";

    private bool isBlinking = true;

    void Start()
    {
        if (inputField == null) inputField = GetComponent<TMP_InputField>();

        if (inputField != null)
        {
            inputField.textComponent.color = new Color(1, 1, 1, 0);
            inputField.customCaretColor = true;
            inputField.caretColor = new Color(1, 1, 1, 0);

            if (inputField.placeholder != null)
            {
                inputField.placeholder.gameObject.SetActive(false);
            }
        }

        StartCoroutine(BlinkCursor());
    }

    void Update()
    {
        if (inputField == null || displayText == null) return;

        string baseText = string.IsNullOrEmpty(inputField.text) ? placeholderMessage : inputField.text;

        displayText.text = baseText + (isBlinking ? cursorChar : " ");

        displayText.color = Color.white;
    }

    private IEnumerator BlinkCursor()
    {
        while (true)
        {
            isBlinking = !isBlinking;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}