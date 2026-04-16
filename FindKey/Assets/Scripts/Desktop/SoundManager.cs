using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Configuración de Audio")]
    public AudioSource sfxSource;

    [Header("Clips de Sonido")]
    public SoundSettings failedTo;
    public SoundSettings clickSound;
    public SoundSettings closeSound;
    public SoundSettings minimizeSound;
    public SoundSettings sendText;

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
                failedTo?.PlayOn(sfxSource, true);
                break;
            case "click":
                clickSound?.PlayOn(sfxSource, true);
                break;
            case "close":
                closeSound?.PlayOn(sfxSource, true);
                break;
            case "minimize":
                minimizeSound?.PlayOn(sfxSource, true);
                break;
            case "send_text":
                sendText?.PlayOn(sfxSource, true);
                break;
        }
    }
}