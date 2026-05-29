using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip soundToPlay;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void playSound()
    {
        audioSource.PlayOneShot(soundToPlay);
    }   
}
