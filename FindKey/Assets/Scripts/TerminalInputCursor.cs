using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_InputField))]
public class TerminalInputCursor : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El componente InputField real (el invisible)")]
    public TMP_InputField inputField;

    [Tooltip("El TextMeshProUGUI que simulará ser el input visible (FakeTerminalText)")]
    public TextMeshProUGUI displayText;

    [Header("Configuración")]
    public string placeholderMessage = "ESCRIBE AQUÍ";
    public float blinkSpeed = 0.5f;
    public string cursorChar = "_";

    private bool isBlinking = true;
    private int lastCaretPos = -1;
    private Coroutine blinkCoroutine;

    void Start()
    {
        if (inputField == null) inputField = GetComponent<TMP_InputField>();

        if (inputField != null && displayText != null)
        {
            if (inputField.textComponent == displayText)
            {
                Debug.LogError("<color=red>[ERROR]</color> Has asignado el texto nativo del InputField en la ranura 'Display Text'. ¡Debes asignar el nuevo 'FakeTerminalText' que creaste!");
                return;
            }
        }

        if (inputField != null)
        {
            // Hacemos invisible todo el input nativo
            inputField.textComponent.color = new Color(1, 1, 1, 0);
            inputField.customCaretColor = true;
            inputField.caretColor = new Color(1, 1, 1, 0);
            inputField.selectionColor = new Color(1, 1, 1, 0);

            if (inputField.placeholder != null)
            {
                inputField.placeholder.gameObject.SetActive(false);
            }
        }

        if (displayText != null)
        {
            displayText.richText = true;
        }

        blinkCoroutine = StartCoroutine(BlinkCursor());
    }

    void Update()
    {
        if (inputField == null || displayText == null) return;

        string realText = inputField.text;

        int currentCaret = inputField.stringPosition;
        if (currentCaret != lastCaretPos)
        {
            lastCaretPos = currentCaret;
            isBlinking = true;
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkCursor());
        }

        if (string.IsNullOrEmpty(realText))
        {
            string blinkCharr = isBlinking ? cursorChar : $"<color=#00000000>{cursorChar}</color>";
            displayText.text = placeholderMessage + blinkCharr;
            displayText.color = Color.white;
            return;
        }

        int caret = Mathf.Clamp(currentCaret, 0, realText.Length);

        string part1 = realText.Substring(0, caret);
        string part2 = realText.Substring(caret);

        string blinkChar = isBlinking ? cursorChar : $"<color=#00000000>{cursorChar}</color>";

        displayText.text = $"{part1}{blinkChar}{part2}";
        displayText.color = Color.white;
    }

    private IEnumerator BlinkCursor()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkSpeed);
            isBlinking = !isBlinking;
        }
    }
}