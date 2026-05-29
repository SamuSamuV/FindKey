using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        if (blackScreen != null)
        {
            StartCoroutine(GlitchRoutine());
        }
    }

    private IEnumerator GlitchRoutine()
    {
        float elapsedTime = 0f;

        // Mientras no hayamos superado los X segundos...
        while (elapsedTime < glitchDuration)
        {
            // 1. Invertimos el estado de la imagen (Si está encendida se apaga, y viceversa)
            blackScreen.enabled = !blackScreen.enabled;

            // 2. Calculamos un tiempo de espera aleatorio para dar sensación de arritmia y fallo
            float randomWait = Random.Range(minFlickerTime, maxFlickerTime);

            yield return new WaitForSeconds(randomWait);

            elapsedTime += randomWait; // Sumamos el tiempo transcurrido
        }

        // --- QUÉ PASA AL TERMINAR ---
        // Forzamos a que la pantalla se quede completamente negra al final
        blackScreen.enabled = true;

        Debug.Log("<color=red>[FIN]</color> El glitch ha terminado. Cerrando el juego...");

        // (Opcional) Una vez se queda en negro, cierra el juego automáticamente. 
        // Si no quieres que se cierre de golpe, puedes borrar esta línea o ponerle un retraso.
        Application.Quit();
    }
}