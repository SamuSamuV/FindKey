using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootManager : MonoBehaviour
{
    [Header("Ajustes de Escena")]
    public float minBootDuration = 2f;
    public float maxBootDuration = 6f;

    [Header("Audio")]
    public AudioSource bootSource;
    public AudioClip bootSound;

    [Header("Animación de Carga")]
    public RectTransform loadingIndicator;
    public float startPosX = -150f;
    public float endPosX = 150f;
    public float moveSpeed = 150f;

    void Start()
    {
        if (bootSource == null) bootSource = FindAnyObjectByType<AudioSource>();

        if (bootSource != null && bootSound != null)
        {
            bootSource.PlayOneShot(bootSound);
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

    private IEnumerator BootSequence(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene("Login");
    }
}