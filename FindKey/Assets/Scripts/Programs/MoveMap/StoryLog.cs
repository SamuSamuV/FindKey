using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharSoundMapping
{
    [Header("Escribe las letras juntas sin espacios. Ej: aeiou")]
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

    [Header("Sonidos de Tipeo")]
    public AudioSource typingAudioSource;
    public SoundSettings defaultTypingSound;
    public List<CharSoundMapping> specificCharacterSounds;

    private Coroutine typingCoroutine;

    private string currentTargetText = "";
    private System.Action currentCallback = null;

    [HideInInspector]
    public string lastLoadedText = "";

    public void AddLine(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (storyText.text.Length > 0) storyText.text += "\n";
        storyText.text += text;

        UpdateLayout();
    }

    public void SetText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        storyText.text = text;
        UpdateLayout();
    }

    public void AddLineAnimated(string text, System.Action onFinished = null)
    {
        AddLineAnimated(text, 0f, onFinished);
    }

    public void AddLineAnimated(string text, float customSpeed, System.Action onFinished = null)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentTargetText = storyText.text + (storyText.text.Length > 0 ? "\n" : "") + text;
        currentCallback = onFinished;

        typingCoroutine = StartCoroutine(TypewriterRoutine(text, customSpeed, onFinished));
    }

    public void SetTextAnimated(string text)
    {
        SetTextAnimated(text, 0f);
    }

    public void SetTextAnimated(string text, float customSpeed)
    {
        if (!text.Contains("You can't do that here."))
        {
            lastLoadedText = text;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        storyText.text = "";

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
                int endIndex = lineToAdd.IndexOf("s]", i);
                if (endIndex != -1)
                {
                    string possibleNumber = lineToAdd.Substring(i + 1, endIndex - (i + 1));

                    if (float.TryParse(possibleNumber, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float pauseDuration))
                    {
                        yield return new WaitForSeconds(pauseDuration);

                        i = endIndex + 1;
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

        if (specificCharacterSounds != null)
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

        if (defaultTypingSound != null && defaultTypingSound.IsValid())
        {
            PlayCleanTypingSound(defaultTypingSound);
        }
    }

    void PlayCleanTypingSound(SoundSettings sound)
    {
        typingAudioSource.pitch = sound.pitch;
        typingAudioSource.PlayOneShot(sound.clip, sound.volume);
    }

    void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();

        if (scrollView != null)
        {
            scrollView.verticalNormalizedPosition = 0f;
        }
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
}