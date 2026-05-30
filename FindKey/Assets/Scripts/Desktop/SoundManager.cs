/// <summary>
/// Class: SoundManager
/// Description: This script manages the sound effects in the FindKey game. It uses a singleton pattern to ensure that there is only one instance of the SoundManager throughout the game.
///              The script contains an AudioSource for playing sound effects and several SoundSettings for different types of sounds (e.g., failedTo, clickSound, closeSound, minimizeSound, sendText).
///              The Play method takes a string parameter to determine which sound to play based on the name provided. Each sound is played using the PlayOn method of the SoundSettings class,
///              which allows for easy configuration of volume and pitch for each sound effect.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } // Singleton instance

    [Header("Configuración de Audio")]
    public AudioSource sfxSource;

    // Different sound settings for various actions in the game
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

    public void Play(string name) // This method can be called from anywhere to play a sound effect by name
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