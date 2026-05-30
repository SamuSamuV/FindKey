using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class: BootManager
/// Description: This script manages the boot sequence of the FindKey game. It handles the display of a loading screen with an animated indicator and plays a boot sound effect.
///              The duration of the boot sequence is randomized between a minimum and maximum value to create variability in the loading experience. Once the boot sequence is complete,
///              it transitions to the "Login" scene. The script also ensures that the loading indicator moves smoothly across the screen during the boot process.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class BootManager : MonoBehaviour
{
    [Header("Ajustes de Escena")]
    public float minBootDuration = 2f;
    public float maxBootDuration = 6f;

    [Header("Audio")]
    public AudioSource bootSource;
    public SoundSettings bootSound;

    [Header("Animación de Carga")]
    public RectTransform loadingIndicator;
    public float startPosX = -150f;
    public float endPosX = 150f;
    public float moveSpeed = 150f;

    void Start()
    {
        if (bootSource == null) bootSource = FindAnyObjectByType<AudioSource>();

        if (bootSource != null && bootSound.IsValid())
        {
            bootSound.PlayOn(bootSource, true);
        }

        float randomDuration = Random.Range(minBootDuration, maxBootDuration);
        StartCoroutine(BootSequence(randomDuration));
    }

    void Update()
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.anchoredPosition += Vector2.right * moveSpeed * Time.deltaTime;

            if (loadingIndicator.anchoredPosition.x > endPosX)
            {
                loadingIndicator.anchoredPosition = new Vector2(startPosX, loadingIndicator.anchoredPosition.y);
            }
        }
    }

    private IEnumerator BootSequence(float duration) // This coroutine manages the boot sequence, waiting for the specified duration before transitioning to the next scene.
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene("Login");
    }
}