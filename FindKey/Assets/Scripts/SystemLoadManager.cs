using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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

    private void Start()
    {
        if (ambientSource == null) ambientSource = GetComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        if (baseFanSound != null && baseFanSound.IsValid())
        {
            baseFanSound.PlayOn(ambientSource, false);
        }

        StartCoroutine(LoadCheckRoutine());
    }

    private IEnumerator LoadCheckRoutine()
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

    private void TriggerSystemStruggle()
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

    private IEnumerator CursorLoadingRoutine(float duration)
    {
        if (CustomCursorManager.Instance != null)
        {
            CustomCursorManager.Instance.isLoading = true;
            yield return new WaitForSeconds(duration);
            CustomCursorManager.Instance.isLoading = false;
        }
    }

    private IEnumerator CooldownRoutine(float time)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(time);
        isOnCooldown = false;
    }
}