/// <summary>
/// Class: PlaySound
/// Description: This class is responsible for playing a sound effect when a specific event occurs in the game. It uses an AudioSource component to play the sound clip assigned to it.
///              The sound can be triggered by calling the playSound() method, which will play the assigned AudioClip once.
///              This class can be attached to any GameObject in the Unity scene that requires sound effects, such as doors opening, keys being collected, or other interactive elements.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip soundToPlay;
    [SerializeField] private AudioSource audioSource;

    void Start() // This method is called before the first frame update
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
