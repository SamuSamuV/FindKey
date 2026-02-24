using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Configuración de Audio")]
    public AudioSource sfxSource;

    [Header("Clips de Sonido")]
    public AudioClip failedTo;
    public AudioClip clickSound;
    public AudioClip closeSound;
    public AudioClip minimizeSound;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void Play(string name)
    {
        if (sfxSource == null) return;

        switch (name.ToLower())
        {
            case "failedto":
                if (failedTo != null) sfxSource.PlayOneShot(failedTo);
                break;
            case "click":
                if (clickSound != null) sfxSource.PlayOneShot(clickSound);
                break;
            case "close":
                if (closeSound != null) sfxSource.PlayOneShot(closeSound);
                break;
            case "minimize":
                if (minimizeSound != null) sfxSource.PlayOneShot(minimizeSound);
                break;
        }
    }
}