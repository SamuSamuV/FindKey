using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class: GlitchScreenEffect
/// Description: Toggles a full-screen black image on and off to create a glitch/flickerscreen effect for a configurable duration, then quits the application.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class GlitchScreenEffect : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("La imagen negra que va a parpadear ocupando toda la pantalla.")]
    public Image blackScreen;

    [Header("Configuración del Glitch")]
    [Tooltip("Tiempo total (en segundos) que durará el efecto de parpadeo.")]
    public float glitchDuration = 5f;
    [Tooltip("Tiempo MÍNIMO entre parpadeos.")]
    public float minFlickerTime = 0.05f;
    [Tooltip("Tiempo MÁXIMO entre parpadeos.")]
    public float maxFlickerTime = 0.25f;

    // Start the glitch coroutine if a blackScreen Image has been assigned
    private void Start()
    {
        if (blackScreen != null)
        {
            StartCoroutine(GlitchRoutine());
        }
    }

    // Coroutine that toggles the blackScreen on/off at random intervals
    // until the configured glitchDuration has passed, then ensures the
    // screen is black and exits the application.
    private IEnumerator GlitchRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < glitchDuration)
        {
            // Toggle the visibility of the black screen to create a flicker
            blackScreen.enabled = !blackScreen.enabled;

            // Wait a random short interval between flickers
            float randomWait = Random.Range(minFlickerTime, maxFlickerTime);

            yield return new WaitForSeconds(randomWait);

            elapsedTime += randomWait;
        }

        // Ensure the screen is black at the end of the effect
        blackScreen.enabled = true;

        Debug.Log("<color=red>[FIN]</color> El glitch ha terminado. Cerrando el juego...");

        // Quit the application (has effect in standalone builds)
        Application.Quit();
    }
}