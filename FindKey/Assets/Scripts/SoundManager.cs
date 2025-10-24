using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioSource sfxSource;
    public AudioClip failedTo;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }


    public void Play(string name)
    {
        if (name == "failedto" && failedTo != null)
        {
            sfxSource.PlayOneShot(failedTo);
        }
    }
}