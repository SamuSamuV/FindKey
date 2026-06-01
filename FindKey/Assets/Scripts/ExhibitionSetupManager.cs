using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExhibitionSetupManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("La caja de texto donde tú (el desarrollador) escribirás el contexto.")]
    public TMP_InputField inputContexto;

    [Tooltip("El botón para arrancar el juego real.")]
    public Button botonIniciar;

    [Tooltip("El nombre de la escena a la que irá (ej: Boot o Login).")]
    public string nombreSiguienteEscena = "Boot";

    private void Start()
    {
        PlayerPrefs.SetString("ContextoFisicoJugador", "");
        PlayerPrefs.Save();

        if (botonIniciar != null)
        {
            botonIniciar.onClick.AddListener(GuardarYContinuar);
        }
    }

    private void GuardarYContinuar()
    {
        if (inputContexto != null && !string.IsNullOrEmpty(inputContexto.text))
        {
            PlayerPrefs.SetString("ContextoFisicoJugador", inputContexto.text);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(nombreSiguienteEscena);
    }
}