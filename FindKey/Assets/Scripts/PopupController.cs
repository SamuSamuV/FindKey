using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
/// <summary>
/// Class: PopupController
/// Description: Controller for handling the display and behavior of pop-up messages in the FindKey project. This class manages the content, appearance, and timing of pop-ups,
///              allowing for dynamic updates based on provided data. It also handles audio playback for associated sounds and ensures proper cleanup after the pop-up is closed.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class PopupController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image iconImage;

    private AudioSource _audioSource;

    void Awake() // Initialize the audio source and set the parent to the canvas
    {
        _audioSource = GetComponent<AudioSource>();

        GameObject container = GameObject.FindGameObjectWithTag("DeskopCanvas");
        if (container != null)
        {
            transform.SetParent(container.transform, false);
        }
    }

    public void Setup(PopupData data) // Configure the pop-up based on the provided data
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

        if (data.sound != null && data.sound.IsValid() && _audioSource != null)
        {
            data.sound.PlayOn(_audioSource, true);
        }

        if (data.duration > 0)
        {
            StartCoroutine(AutoCloseRoutine(data.duration));
        }
    }

    IEnumerator AutoCloseRoutine(float time) // Coroutine to automatically close the pop-up after a specified duration
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void ClosePopup() // Method to manually close the pop-up, can be called from UI buttons or other events
    {
        Destroy(gameObject);
    }
}