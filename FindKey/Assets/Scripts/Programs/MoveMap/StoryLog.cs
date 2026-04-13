using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharSoundMapping
{
    public string characters;
    public SoundSettings sound;
}

public class StoryLog : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text storyText;
    public RectTransform contentRect;
    public ScrollRect scrollView;

    [Header("Configuración")]
    public float typingSpeed = 0.03f;

    [Header("Sonidos de Tipeo (IA / Letras)")]
    public AudioSource typingAudioSource;
    public SoundSettings defaultTypingSound;
    public List<CharSoundMapping> specificCharacterSounds;

    [Header("Sonidos de Narrativa (Aventura)")]
    public List<SoundSettings> narrativeSounds;
    [Range(0f, 0.5f)] public float pitchVariation = 0.1f;
    [Range(0f, 0.2f)] public float volumeVariation = 0.05f;

    [Header("Efectos Visuales de Emoción")]
    public float happyShakeMagnitude = 1f;
    public float happyShakeSpeed = 10f;

    public float sadShakeMagnitude = 0.5f;
    public float sadShakeSpeed = 2f;

    public float neutralShakeMagnitude = 0f;
    public float neutralShakeSpeed = 0f;

    private float currentShakeMagnitude = 0f;
    private float currentShakeSpeed = 0f;

    private int wobbleStartIndex = 0;
    private int wobbleEndIndex = -1;

    private Coroutine typingCoroutine;
    private string currentTargetText = "";
    private System.Action currentCallback = null;

    [HideInInspector]
    public string lastLoadedText = "";

    void Update()
    {
        if (currentShakeMagnitude <= 0f || storyText == null) return;

        storyText.ForceMeshUpdate();
        TMP_TextInfo textInfo = storyText.textInfo;

        int endIndex = wobbleEndIndex == -1 ? textInfo.characterCount : wobbleEndIndex;
        endIndex = Mathf.Min(endIndex, textInfo.characterCount);

        for (int i = wobbleStartIndex; i < endIndex; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

            float offsetX = 0f;
            float offsetY = Mathf.Sin(Time.time * currentShakeSpeed + (i * 12.3f)) * currentShakeMagnitude;
            Vector3 offset = new Vector3(offsetX, offsetY, 0);

            sourceVertices[vertexIndex + 0] += offset;
            sourceVertices[vertexIndex + 1] += offset;
            sourceVertices[vertexIndex + 2] += offset;
            sourceVertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            storyText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    public void SetEmotion(NPCVisualController.NPCEmotion emo)
    {
        switch (emo)
        {
            case NPCVisualController.NPCEmotion.Happy:
                currentShakeMagnitude = happyShakeMagnitude;
                currentShakeSpeed = happyShakeSpeed;
                break;
            case NPCVisualController.NPCEmotion.Sad:
                currentShakeMagnitude = sadShakeMagnitude;
                currentShakeSpeed = sadShakeSpeed;
                break;
            case NPCVisualController.NPCEmotion.Neutral:
            default:
                currentShakeMagnitude = neutralShakeMagnitude;
                currentShakeSpeed = neutralShakeSpeed;
                break;
        }
    }

    public void StopEmotion()
    {
        currentShakeMagnitude = 0f;
        currentShakeSpeed = 0f;
        if (storyText != null) storyText.ForceMeshUpdate();
    }

    public void AddLine(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        storyText.ForceMeshUpdate();
        if (wobbleEndIndex == -1)
        {
            wobbleEndIndex = storyText.textInfo.characterCount;
        }

        if (storyText.text.Length > 0) storyText.text += "\n";
        storyText.text += text;
        UpdateLayout();
    }

    public void SetText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        wobbleStartIndex = 0;
        wobbleEndIndex = text.Length;

        storyText.text = text;
        UpdateLayout();
    }

    public void AddLineAnimated(string text, System.Action onFinished = null) => AddLineAnimated(text, 0f, onFinished);

    public void AddLineAnimated(string text, float customSpeed, System.Action onFinished = null)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        storyText.ForceMeshUpdate();
        wobbleStartIndex = storyText.textInfo.characterCount;
        wobbleEndIndex = -1;

        currentTargetText = storyText.text + (storyText.text.Length > 0 ? "\n" : "") + text;
        currentCallback = onFinished;
        typingCoroutine = StartCoroutine(TypewriterRoutine(text, customSpeed, onFinished));
    }

    public void SetTextAnimated(string text) => SetTextAnimated(text, 0f);

    public void SetTextAnimated(string text, float customSpeed)
    {
        if (!text.Contains("You can't do that here.")) lastLoadedText = text;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        storyText.text = "";

        wobbleStartIndex = 0;
        wobbleEndIndex = -1;

        currentTargetText = text;
        currentCallback = null;
        typingCoroutine = StartCoroutine(TypewriterRoutine(text, customSpeed));
    }

    IEnumerator TypewriterRoutine(string lineToAdd, float customSpeed, System.Action onFinished = null)
    {
        if (storyText.text.Length > 0) storyText.text += "\n";
        bool isInsideTag = false;
        float activeSpeed = customSpeed > 0f ? customSpeed : typingSpeed;

        for (int i = 0; i < lineToAdd.Length; i++)
        {
            if (lineToAdd[i] == '[')
            {
                int endIndexTag = lineToAdd.IndexOf("s]", i);
                if (endIndexTag != -1)
                {
                    string possibleNumber = lineToAdd.Substring(i + 1, endIndexTag - (i + 1));
                    if (float.TryParse(possibleNumber, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float pauseDuration))
                    {
                        yield return new WaitForSeconds(pauseDuration);
                        i = endIndexTag + 1;
                        continue;
                    }
                }
            }

            char c = lineToAdd[i];
            if (c == '<') isInsideTag = true;
            storyText.text += c;
            if (c == '>') isInsideTag = false;

            if (!isInsideTag)
            {
                PlayTypingSound(c);
                UpdateLayout();
                yield return new WaitForSeconds(activeSpeed);
            }
        }

        typingCoroutine = null;
        currentCallback = null;
        onFinished?.Invoke();
    }

    void PlayTypingSound(char c)
    {
        if (typingAudioSource == null) return;
        if (char.IsWhiteSpace(c)) return;

        string charStr = c.ToString().ToLowerInvariant();

        if (specificCharacterSounds != null && specificCharacterSounds.Count > 0)
        {
            foreach (var mapping in specificCharacterSounds)
            {
                if (!string.IsNullOrEmpty(mapping.characters) && mapping.characters.ToLowerInvariant().Contains(charStr))
                {
                    if (mapping.sound != null && mapping.sound.IsValid())
                    {
                        PlayCleanTypingSound(mapping.sound);
                        return;
                    }
                }
            }
        }

        if (narrativeSounds != null && narrativeSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, narrativeSounds.Count);
            PlayCleanTypingSound(narrativeSounds[randomIndex]);
        }
        else if (defaultTypingSound != null && defaultTypingSound.IsValid())
        {
            PlayCleanTypingSound(defaultTypingSound);
        }
    }

    void PlayCleanTypingSound(SoundSettings sound)
    {
        float randomPitch = Random.Range(-pitchVariation, pitchVariation);
        typingAudioSource.pitch = sound.pitch + randomPitch;

        float randomVol = Random.Range(-volumeVariation, volumeVariation);
        float finalVol = Mathf.Clamp01(sound.volume + randomVol);

        typingAudioSource.PlayOneShot(sound.clip, finalVol);
    }

    void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        if (scrollView != null) scrollView.verticalNormalizedPosition = 0f;
    }

    private void OnDisable()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            string cleanedText = System.Text.RegularExpressions.Regex.Replace(currentTargetText, @"\[\d+(\.\d+)?s\]", "");
            storyText.text = cleanedText;
            UpdateLayout();
            currentCallback?.Invoke();
            currentCallback = null;
        }
    }

    [Space(20)]
    [Header("Herramienta de Ajuste Global")]
    public SoundSettings parametrosGlobales;

    [ContextMenu("¡Aplicar parámetros a TODO el abecedario!")]
    public void AplicarParametrosATodas()
    {
        if (parametrosGlobales == null) return;
        int contador = 0;
        if (defaultTypingSound != null)
        {
            defaultTypingSound.volume = parametrosGlobales.volume;
            defaultTypingSound.pitch = parametrosGlobales.pitch;
            defaultTypingSound.loop = parametrosGlobales.loop;
            defaultTypingSound.fadeInDuration = parametrosGlobales.fadeInDuration;
            defaultTypingSound.fadeOutDuration = parametrosGlobales.fadeOutDuration;
        }

        if (specificCharacterSounds != null)
        {
            foreach (var mapping in specificCharacterSounds)
            {
                if (mapping.sound != null)
                {
                    mapping.sound.volume = parametrosGlobales.volume;
                    mapping.sound.pitch = parametrosGlobales.pitch;
                    mapping.sound.loop = parametrosGlobales.loop;
                    mapping.sound.fadeInDuration = parametrosGlobales.fadeInDuration;
                    mapping.sound.fadeOutDuration = parametrosGlobales.fadeOutDuration;
                    contador++;
                }
            }
        }

        Debug.Log($"<color=green>¡Éxito!</color> Se han ajustado {contador} sonidos.");
    }
}