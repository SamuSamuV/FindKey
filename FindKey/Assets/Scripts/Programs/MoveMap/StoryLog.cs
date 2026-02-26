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
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentTargetText = storyText.text + (storyText.text.Length > 0 ? "\n" : "") + text;
        currentCallback = onFinished;

        typingCoroutine = StartCoroutine(TypewriterRoutine(text, onFinished));
    }

    IEnumerator TypewriterRoutine(string lineToAdd, System.Action onFinished = null)
    {
        if (storyText.text.Length > 0) storyText.text += "\n";

        bool isInsideTag = false;

        for (int i = 0; i < lineToAdd.Length; i++)
        {
            char c = lineToAdd[i];

            if (c == '<') isInsideTag = true;
            storyText.text += c;
            if (c == '>') isInsideTag = false;

            if (!isInsideTag)
            {
                PlayTypingSound(c);
                UpdateLayout();
                yield return new WaitForSeconds(typingSpeed);
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
                        mapping.sound.PlayOn(typingAudioSource, true);
                        return;
                    }
                }
            }
        }

        if (defaultTypingSound != null && defaultTypingSound.IsValid())
        {
            defaultTypingSound.PlayOn(typingAudioSource, true);
        }
    }

    void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        if (scrollView != null) scrollView.verticalNormalizedPosition = 0f;
    }

    public void SetTextAnimated(string text)
    {
        if (!text.Contains("You can't do that here."))
        {
            lastLoadedText = text;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        storyText.text = "";

        currentTargetText = text;
        currentCallback = null;

        typingCoroutine = StartCoroutine(TypewriterRoutine(text));
    }

    private void OnDisable()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            storyText.text = currentTargetText;
            UpdateLayout();

            currentCallback?.Invoke();
            currentCallback = null;
        }
    }
}