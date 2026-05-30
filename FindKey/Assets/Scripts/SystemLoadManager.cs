using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

/// <summary>
/// Class: SystemLoadManager
/// Description: Manages the simulation of system load based on the number of open windows in the Taskbar. As the load increases, it has a chance to trigger "struggle" events,
///              which play specific sounds and change the cursor to indicate the system is under stress. The manager also includes cooldown mechanics to prevent constant triggering ofstruggle events.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class SystemLoadManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource sfxSource;

    [Header("Sonidos")]
    public SoundSettings baseFanSound;
    public List<SoundSettings> heavyLoadSounds;

    [Header("Configuración de Carga")]
    public int maxWindowsForStress = 7;

    public float minCheckInterval = 15f;
    public float maxCheckInterval = 30f;

    [Range(0f, 1f)]
    public float maxStruggleChance = 0.4f;

    public float cooldownAfterStruggle = 20f;

    private bool isOnCooldown = false;

    private void Start() // Initialize audio sources and start the load checking routine
    {
        if (ambientSource == null) ambientSource = GetComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        if (baseFanSound != null && baseFanSound.IsValid())
        {
            baseFanSound.PlayOn(ambientSource, false);
        }

        StartCoroutine(LoadCheckRoutine());
    }

    private IEnumerator LoadCheckRoutine() // Periodically checks the number of open windows and determines if a struggle event should be triggered based on the load percentage
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minCheckInterval, maxCheckInterval));

            if (isOnCooldown) continue;

            int openWindows = TaskbarManager.GetOrFindInstance() != null ? TaskbarManager.GetOrFindInstance().OpenAppCount : 0;

            if (openWindows > 0)
            {
                float loadPercentage = Mathf.Clamp01((float)openWindows / maxWindowsForStress);

                float chanceOfStruggle = loadPercentage * maxStruggleChance;

                if (Random.value < chanceOfStruggle)
                {
                    TriggerSystemStruggle();
                }
            }
        }
    }

    private void TriggerSystemStruggle() // Plays a random struggle sound and initiates the cursor loading effect and cooldown
    {
        if (heavyLoadSounds != null && heavyLoadSounds.Count > 0)
        {
            SoundSettings randomStruggleSound = heavyLoadSounds[Random.Range(0, heavyLoadSounds.Count)];

            if (randomStruggleSound.IsValid())
            {
                randomStruggleSound.PlayOn(sfxSource, true);

                float duration = randomStruggleSound.clip.length / Mathf.Max(randomStruggleSound.pitch, 0.01f);

                StartCoroutine(CursorLoadingRoutine(duration));

                StartCoroutine(CooldownRoutine(duration + cooldownAfterStruggle));
            }
        }
    }

    private IEnumerator CursorLoadingRoutine(float duration) // Simulates a loading cursor effect for the duration of the struggle sound, indicating that the system is under stress
    {
        if (CustomCursorManager.Instance != null)
        {
            CustomCursorManager.Instance.isLoading = true;
            yield return new WaitForSeconds(duration);
            CustomCursorManager.Instance.isLoading = false;
        }
    }

    private IEnumerator CooldownRoutine(float time) // Puts the system on cooldown for a specified duration to prevent immediate retriggering of struggle events, allowing the player to recover from the simulated system stress
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(time);
        isOnCooldown = false;
    }
}