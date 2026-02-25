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

    public float minCheckInterval = 3f;
    public float maxCheckInterval = 8f;

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

            int openWindows = TaskbarManager.GetOrFindInstance() != null ? TaskbarManager.GetOrFindInstance().OpenAppCount : 0;

            float loadPercentage = Mathf.Clamp01((float)openWindows / maxWindowsForStress);

            float chanceOfStruggle = loadPercentage * 0.85f;

            if (openWindows > 0 && Random.value < chanceOfStruggle)
            {
                TriggerSystemStruggle();
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
}