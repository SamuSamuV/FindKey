using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("Referencias Originales")]
    public Button accountButton;
    public TextMeshProUGUI welcomeText;
    public float welcomeDuration = 3f;

    [Header("Efecto Fade-In Inicial")]
    public TextMeshProUGUI textoAparecer; // El texto que aparecerá primero
    public CanvasGroup botonAparecer;     // El CanvasGroup del botón que aparecerá después
    public float textFadeDuration = 1.5f; // Segundos que tarda en aparecer el texto
    public float buttonFadeDuration = 1.5f; // Segundos que tarda en aparecer el botón

    private void Start()
    {
        if (welcomeText) welcomeText.gameObject.SetActive(false);
        if (accountButton != null) accountButton.onClick.AddListener(OnAccountClicked);

        // Arrancamos la animación al empezar la escena
        StartCoroutine(FadeInRutina());
    }

    private IEnumerator FadeInRutina()
    {
        // 1. Preparamos los elementos dejándolos invisibles (Alpha = 0)
        if (textoAparecer != null)
        {
            Color c = textoAparecer.color;
            c.a = 0f;
            textoAparecer.color = c;
        }

        if (botonAparecer != null)
        {
            botonAparecer.alpha = 0f;
            botonAparecer.interactable = false; // Evita clicks accidentales mientras es invisible
            botonAparecer.blocksRaycasts = false;
        }

        // 2. Animamos la aparición del TEXTO
        if (textoAparecer != null)
        {
            float tiempo = 0f;
            while (tiempo < textFadeDuration)
            {
                tiempo += Time.deltaTime;
                Color c = textoAparecer.color;
                c.a = Mathf.Lerp(0f, 1f, tiempo / textFadeDuration);
                textoAparecer.color = c;
                yield return null;
            }
        }

        // (Opcional) Una pequeña pausa de medio segundo antes de mostrar el botón
        yield return new WaitForSeconds(0.5f);

        // 3. Animamos la aparición del BOTÓN
        if (botonAparecer != null)
        {
            float tiempo = 0f;
            while (tiempo < buttonFadeDuration)
            {
                tiempo += Time.deltaTime;
                botonAparecer.alpha = Mathf.Lerp(0f, 1f, tiempo / buttonFadeDuration);
                yield return null;
            }
            // Lo activamos del todo cuando ya es visible
            botonAparecer.interactable = true;
            botonAparecer.blocksRaycasts = true;
        }
    }

    private void OnAccountClicked()
    {
        if (welcomeText) welcomeText.gameObject.SetActive(true);

        StartCoroutine(GoToDesktop());
    }

    private IEnumerator GoToDesktop()
    {
        yield return new WaitForSeconds(welcomeDuration);
        SceneManager.LoadScene("Desktop");
    }
}