using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class: LoginManager
/// Description: This script manages the login screen of the FindKey game. It handles the initial fade-in animation of the welcome text and the account button, creating a smooth and
///              engaging user experience. When the player clicks the account button, it displays a welcome message and then transitions to the desktop scene after a short delay. The script uses
///              coroutines to manage the timing of the animations and scene transition, ensuring that everything flows seamlessly for the player.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class LoginManager : MonoBehaviour
{
    [Header("Referencias Originales")]
    public Button accountButton;
    public TextMeshProUGUI welcomeText;
    public float welcomeDuration = 3f;

    [Header("Efecto Fade-In Inicial")]
    public TextMeshProUGUI textoAparecer; // The text that will appear first
    public CanvasGroup botonAparecer;     // The CanvasGroup of the button that will appear after
    public float textFadeDuration = 1.5f; // Seconds it takes for the text to appear
    public float buttonFadeDuration = 1.5f; // Seconds it takes for the button to appear

    private void Start()
    {
        if (welcomeText) welcomeText.gameObject.SetActive(false);
        if (accountButton != null) accountButton.onClick.AddListener(OnAccountClicked);

        // Start the animation when the scene begins
        StartCoroutine(FadeInRutina());
    }

    private IEnumerator FadeInRutina() // Routine to handle the fade-in animation of the text and button
    {
        // 1. Prepare elements by making them invisible (Alpha = 0)
        if (textoAparecer != null)
        {
            Color c = textoAparecer.color;
            c.a = 0f;
            textoAparecer.color = c;
        }

        if (botonAparecer != null)
        {
            botonAparecer.alpha = 0f;
            botonAparecer.interactable = false; // Disable interaction until it is fully visible
            botonAparecer.blocksRaycasts = false; // Prevent it from blocking clicks while invisible
        }

        // 2. Animate the TEXT appearance
        if (textoAparecer != null)
        {
            float tiempo = 0f;
            while (tiempo < textFadeDuration) // Loop until the text is fully visible
            {
                tiempo += Time.deltaTime;
                Color c = textoAparecer.color;
                c.a = Mathf.Lerp(0f, 1f, tiempo / textFadeDuration);
                textoAparecer.color = c;
                yield return null;
            }
        }

        // (Optional) A short half-second pause before showing the button
        yield return new WaitForSeconds(0.5f);

        // 3. Animate the BUTTON appearance
        if (botonAparecer != null)
        {
            float tiempo = 0f;
            while (tiempo < buttonFadeDuration)
            {
                tiempo += Time.deltaTime;
                botonAparecer.alpha = Mathf.Lerp(0f, 1f, tiempo / buttonFadeDuration);
                yield return null;
            }
            // Fully enable it when it is visible
            botonAparecer.interactable = true;
            botonAparecer.blocksRaycasts = true;
        }
    }

    private void OnAccountClicked() // This function is called when the account button is clicked
    {
        if (welcomeText) welcomeText.gameObject.SetActive(true);

        StartCoroutine(GoToDesktop());
    }

    private IEnumerator GoToDesktop() // Routine to handle the delay before transitioning to the desktop scene
    {
        yield return new WaitForSeconds(welcomeDuration);
        SceneManager.LoadScene("Desktop");
    }
}