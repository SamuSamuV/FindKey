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

    public void Setup(PopupData data)
    {
        if (titleText != null) titleText.text = data.title;
        if (bodyText != null) bodyText.text = data.message;

        if (iconImage != null)
        {
            if (data.image != null)
            {
                iconImage.sprite = data.image;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
        }

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

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}