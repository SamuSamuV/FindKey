using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PopupController : MonoBehaviour
{
    [Header("Referencias UI (Arrastrar en el Prefab)")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image iconImage;

    // Método que llamará el AdventureManager para inyectar los datos
    public void Setup(PopupData data)
    {
        // 1. Configurar Textos
        if (titleText != null) titleText.text = data.title;
        if (bodyText != null) bodyText.text = data.message;

        // 2. Configurar Imagen
        if (iconImage != null)
        {
            if (data.image != null)
            {
                iconImage.sprite = data.image;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                // Si no hay imagen, desactivamos el componente Image para que no se vea un cuadro blanco
                iconImage.gameObject.SetActive(false);
            }
        }

        // 3. Configurar Auto-Cierre
        if (data.duration > 0)
        {
            StartCoroutine(AutoCloseRoutine(data.duration));
        }
    }

    IEnumerator AutoCloseRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        ClosePopup();
    }

    // Vincula este método a tu botón de cerrar en el Inspector
    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}