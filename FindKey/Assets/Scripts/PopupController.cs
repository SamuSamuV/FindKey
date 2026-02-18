using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PopupController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image iconImage;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        GameObject container = GameObject.FindGameObjectWithTag("DeskopCanvas");
        if (container != null)
        {
            transform.SetParent(container.transform, false);
        }
    }

    public void Setup(PopupData data)
    {
        if (titleText != null) titleText.text = data.title;
        if (bodyText != null) bodyText.text = data.message;

        if (iconImage != null)
        {
            iconImage.sprite = data.image;
            iconImage.gameObject.SetActive(data.image != null);
        }

        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = data.position;
        }

        if (data.sound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(data.sound);
        }

        if (data.duration > 0)
        {
            StartCoroutine(AutoCloseRoutine(data.duration));
        }
    }

    IEnumerator AutoCloseRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}