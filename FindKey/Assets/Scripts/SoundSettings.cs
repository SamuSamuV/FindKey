using System.Collections;
using UnityEngine;

[System.Serializable]
public class SoundSettings
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop = false;

    [Header("Efectos de Transicion (Segundos)")]
    public float fadeInDuration = 0f;
    public float fadeOutDuration = 0f;

    public bool IsValid() => clip != null;

    public void PlayOn(AudioSource baseSource, bool oneShot = false)
    {
        if (!IsValid() || baseSource == null) return;

        if (oneShot)
        {
            AudioSource tempSource = baseSource.gameObject.AddComponent<AudioSource>();

            tempSource.spatialBlend = baseSource.spatialBlend;
            tempSource.outputAudioMixerGroup = baseSource.outputAudioMixerGroup;
            tempSource.clip = clip;
            tempSource.pitch = pitch;
            tempSource.loop = false;

            AudioFader fader = tempSource.gameObject.AddComponent<AudioFader>();
            fader.StartFade(tempSource, volume, fadeInDuration, fadeOutDuration, true);
        }

        else
        {
            baseSource.clip = clip;
            baseSource.pitch = pitch;
            baseSource.loop = loop;

            AudioFader fader = baseSource.GetComponent<AudioFader>();
            if (fader == null) fader = baseSource.gameObject.AddComponent<AudioFader>();

            fader.StartFade(baseSource, volume, fadeInDuration, fadeOutDuration, false);
        }
    }
}

public class AudioFader : MonoBehaviour
{
    private Coroutine currentFade;
    public float storedFadeOutTime = 0f;
    private AudioSource mySource;

    public void StartFade(AudioSource source, float targetVol, float fadeIn, float fadeOut, bool isOneShot)
    {
        mySource = source;
        storedFadeOutTime = fadeOut;

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(source, targetVol, fadeIn, fadeOut, isOneShot));
    }

    private IEnumerator FadeRoutine(AudioSource source, float targetVol, float fadeIn, float fadeOut, bool isOneShot)
    {
        if (fadeIn > 0f)
        {
            source.volume = 0f;
            source.Play();
            float t = 0f;

            while (t < fadeIn)
            {
                t += Time.deltaTime;
                source.volume = Mathf.Lerp(0f, targetVol, t / fadeIn);
                yield return null;
            }
        }

        source.volume = targetVol;
        if (!source.isPlaying) source.Play();

        if (isOneShot)
        {
            float totalDuration = source.clip.length / Mathf.Max(source.pitch, 0.01f);
            float waitTime = totalDuration - fadeIn - fadeOut;

            if (waitTime > 0) yield return new WaitForSeconds(waitTime);

            if (fadeOut > 0f)
            {
                float t = 0f;

                while (t < fadeOut)
                {
                    t += Time.deltaTime;
                    source.volume = Mathf.Lerp(targetVol, 0f, t / fadeOut);
                    yield return null;
                }
            }

            source.volume = 0f;
            Destroy(source);
            Destroy(this);
        }
    }

    public void FadeOutAndDestroyGameObject()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        StartCoroutine(FadeOutDestroyGameObjectRoutine());
    }

    private IEnumerator FadeOutDestroyGameObjectRoutine()
    {
        if (mySource != null && storedFadeOutTime > 0f && mySource.isPlaying)
        {
            float startVol = mySource.volume;
            float t = 0f;

            while (t < storedFadeOutTime)
            {
                t += Time.deltaTime;
                mySource.volume = Mathf.Lerp(startVol, 0f, t / storedFadeOutTime);
                yield return null;
            }
        }

        if (mySource != null)
        {
            mySource.Stop();
        }

        Destroy(gameObject);
    }
}