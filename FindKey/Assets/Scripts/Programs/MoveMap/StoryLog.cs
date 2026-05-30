

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class: CharSoundMapping
/// Description: This class is a simple data structure used in the StoryLog script to map specific characters to their corresponding typing sound settings.
///              It contains a string of characters and a SoundSettings object that defines the audio properties for those characters. This allows for more granular control over the typing sounds,
///              enabling different sounds for different types of characters (e.g., vowels, consonants, punctuation) in the FindKey game.
/// </summary>
[System.Serializable]
public class CharSoundMapping
{
    public string characters;
    public SoundSettings sound;
}

/// <summary>
/// File: StoryLog
/// Description: This script manages the story log in the FindKey game, providing functionalities for displaying text with a typewriter effect, playing typing sounds,
///              and adding visual effects based on the NPC's emotion. It allows for adding lines of text either instantly or with animation, and supports skipping the typing effect if desired.
///              The script also includes a tool for applying global sound parameters to all character-specific typing sounds, making it easier to maintain consistency across the game's audio design.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class StoryLog : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text storyText;
    public RectTransform contentRect;
    public ScrollRect scrollView;

    [Header("Configuración")]
    public float typingSpeed = 0.03f;

    [Header("Sonidos de Tipeo (IA / Letras)")]
    public AudioSource typingAudioSource; // This AudioSource is used as a template for playing typing sounds. When a typing sound is played, a temporary AudioSource is created based on this template, allowing for multiple overlapping sounds without interrupting each other. The script ensures that the temporary AudioSource has the same spatial blend and output mixer group as the original, maintaining consistency in audio settings across all typing sounds.
    public SoundSettings defaultTypingSound; // This SoundSettings object defines the default audio properties for typing sounds. If a specific character does not have its own assigned sound in the specificCharacterSounds list, this default sound will be used when that character is typed. It includes properties such as volume, pitch, loop settings, and fade durations, providing a baseline for the audio experience of the typing effect in the FindKey game.
    public List<CharSoundMapping> specificCharacterSounds; // This list allows for mapping specific characters to their own unique typing sounds. Each entry in the list consists of a string of characters and a corresponding SoundSettings object. When a character is typed, the script checks this list to see if there is a specific sound assigned to it. If a match is found, that sound is played instead of the default typing sound. This feature enables more variety and customization in the audio feedback for different types of characters (e.g., vowels, consonants, punctuation) in the FindKey game.

    [Header("Sonidos de Narrativa (Aventura)")]
    public List<SoundSettings> narrativeSounds; // This list contains SoundSettings objects that define the audio properties for narrative typing sounds. When a character is typed and there is no specific sound assigned to it in the specificCharacterSounds list, the script will randomly select one of the sounds from this narrativeSounds list to play. This allows for a more dynamic and varied audio experience during the typing effect, enhancing the immersion in the FindKey game. Each SoundSettings object in this list can have different volume, pitch, loop settings, and fade durations, contributing to a richer auditory atmosphere as the story unfolds.
    [Range(0f, 0.5f)] public float pitchVariation = 0.1f;
    [Range(0f, 0.2f)] public float volumeVariation = 0.05f;

    [Header("Efectos Visuales de Emoción")]
    public float happyShakeMagnitude = 1f;
    public float happyShakeSpeed = 10f;

    public float sadShakeMagnitude = 0.5f;
    public float sadShakeSpeed = 2f;

    public float neutralShakeMagnitude = 0f;
    public float neutralShakeSpeed = 0f;

    [Header("Modificadores de Sonido/Velocidad por Emoción")]
    public float happyPitchMultiplier = 1.1f;
    public float happySpeedMultiplier = 1.1f;

    public float sadPitchMultiplier = 1.2f;
    public float sadSpeedMultiplier = 0.8f;

    public float neutralPitchMultiplier = 1.0f;
    public float neutralSpeedMultiplier = 1.0f;

    private float currentShakeMagnitude = 0f;
    private float currentShakeSpeed = 0f;
    private float currentPitchMultiplier = 1.0f;
    private float currentSpeedMultiplier = 1.0f;

    private int wobbleStartIndex = 0;
    private int wobbleEndIndex = -1;

    private Coroutine typingCoroutine; // Reference to the currently running typewriter coroutine, allowing for control over the typing effect (e.g., stopping it when a new line is added or when the object is disabled).
    private string currentTargetText = ""; // The full text that is currently being typed out, used to ensure that if the typing effect is skipped, the complete text is displayed immediately without any remaining typewriter effect.
    private System.Action currentCallback = null; // An optional callback that can be invoked when the typing effect finishes, allowing for additional actions to be performed after the text has been fully displayed (e.g., enabling UI elements, triggering events).

    [HideInInspector]
    public string lastLoadedText = "";

    [HideInInspector] public bool canSkipCurrentText = true;
    private bool isSkipping = false;

    public bool IsTyping => typingCoroutine != null;

    void Update() // This method applies a wobble effect to the text characters based on the current emotion settings. It iterates through the characters in the text and applies a vertical offset to create a shaking effect.
    {
        if (currentShakeMagnitude <= 0f || storyText == null) return;

        storyText.ForceMeshUpdate();
        TMP_TextInfo textInfo = storyText.textInfo;

        int endIndex = wobbleEndIndex == -1 ? textInfo.characterCount : wobbleEndIndex;
        endIndex = Mathf.Min(endIndex, textInfo.characterCount);

        for (int i = wobbleStartIndex; i < endIndex; i++) // Iterate through the characters in the text, applying a vertical offset to create a shaking effect based on the current emotion settings. The offset is calculated using a sine wave function that varies over time and is influenced by the character's index to create a more dynamic and varied wobble effect across the text.
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

        for (int i = 0; i < textInfo.meshInfo.Length; i++) // After modifying the vertex positions for the wobble effect, this loop updates the mesh for each material used by the text to reflect the changes. It assigns the modified vertices back to the mesh and calls UpdateGeometry to apply the changes visually in the UI.
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            storyText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    public void TrySkipTyping() // This method allows for skipping the current typing effect if it is active and if skipping is allowed for the current text. If the typing coroutine is running and canSkipCurrentText is true, it sets the isSkipping flag to true, which will cause the typewriter routine to immediately display the full text without waiting for the remaining characters to be typed out.
    {
        if (typingCoroutine != null && canSkipCurrentText)
        {
            isSkipping = true;
        }
    }

    public void SetEmotion(NPCVisualController.NPCEmotion emo) // This method sets the current emotion for the story log, which affects the visual shaking effect and the audio properties of the typing sounds. Based on the specified emotion (Happy, Sad, Neutral), it updates the current shake magnitude, shake speed, pitch multiplier, and speed multiplier to create a corresponding visual and auditory experience that matches the NPC's emotional state in the FindKey game.
    {
        switch (emo)
        {
            case NPCVisualController.NPCEmotion.Happy:
                currentShakeMagnitude = happyShakeMagnitude;
                currentShakeSpeed = happyShakeSpeed;
                currentPitchMultiplier = happyPitchMultiplier;
                currentSpeedMultiplier = happySpeedMultiplier;
                break;
            case NPCVisualController.NPCEmotion.Sad:
                currentShakeMagnitude = sadShakeMagnitude;
                currentShakeSpeed = sadShakeSpeed;
                currentPitchMultiplier = sadPitchMultiplier;
                currentSpeedMultiplier = sadSpeedMultiplier;
                break;
            case NPCVisualController.NPCEmotion.Neutral:
            default:
                currentShakeMagnitude = neutralShakeMagnitude;
                currentShakeSpeed = neutralShakeSpeed;
                currentPitchMultiplier = neutralPitchMultiplier;
                currentSpeedMultiplier = neutralSpeedMultiplier;
                break;
        }
    }

    public void StopEmotion() // This method resets the emotion effects on the story log, setting all shake magnitudes, shake speeds, pitch multipliers, and speed multipliers back to their default values. This effectively stops any ongoing visual shaking effects and returns the audio properties of the typing sounds to their normal state, ensuring that the story log returns to a neutral presentation after an emotional effect has been applied in the FindKey game.
    {
        currentShakeMagnitude = 0f;
        currentShakeSpeed = 0f;
        currentPitchMultiplier = 1.0f;
        currentSpeedMultiplier = 1.0f;
        if (storyText != null) storyText.ForceMeshUpdate();
    }

    public void AddLine(string text) // This method adds a new line of text to the story log instantly, without any animation. It first checks if there is an active typing coroutine and stops it if necessary to ensure that the new text is added immediately. Then, it forces an update of the text mesh to ensure that the layout is correct before adding the new line. If the current text is not empty, it adds a newline character before appending the new text. Finally, it calls UpdateLayout to adjust the UI layout and scroll to the bottom of the log to show the newly added line in the FindKey game.
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

    public void SetText(string text) // This method sets the entire text of the story log instantly, replacing any existing content. It first checks if there is an active typing coroutine and stops it if necessary to ensure that the new text is set immediately. Then, it updates the wobble effect indices to encompass the entire new text. Finally, it calls UpdateLayout to adjust the UI layout and scroll to the bottom of the log to show the newly set text in the FindKey game.
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        wobbleStartIndex = 0;
        wobbleEndIndex = text.Length;

        storyText.text = text;
        UpdateLayout();
    }

    public void AddLineAnimated(string text, System.Action onFinished = null) => AddLineAnimated(text, 0f, onFinished); // This method adds a new line of text to the story log with a typewriter animation effect. It allows for an optional callback to be invoked when the typing effect finishes. The method first checks if there is an active typing coroutine and stops it if necessary to ensure that the new line is added with the animation. Then, it forces an update of the text mesh to ensure that the layout is correct before starting the typewriter routine. The new line is appended to the existing text, and the typewriter routine is started with the specified text and callback in the FindKey game.

    public void AddLineAnimated(string text, float customSpeed, System.Action onFinished = null) // This method adds a new line of text to the story log with a typewriter animation effect, allowing for a custom typing speed and an optional callback when the typing finishes. It first checks if there is an active typing coroutine and stops it if necessary to ensure that the new line is added with the animation. Then, it forces an update of the text mesh to ensure that the layout is correct before starting the typewriter routine. The new line is appended to the existing text, and the typewriter routine is started with the specified text, custom speed, and callback in the FindKey game.
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        storyText.ForceMeshUpdate();
        wobbleStartIndex = storyText.textInfo.characterCount;
        wobbleEndIndex = -1;

        currentTargetText = storyText.text + (storyText.text.Length > 0 ? "\n" : "") + text;
        currentCallback = onFinished;
        typingCoroutine = StartCoroutine(TypewriterRoutine(text, customSpeed, onFinished));
    }

    public void SetTextAnimated(string text) => SetTextAnimated(text, 0f); // This method sets the entire text of the story log with a typewriter animation effect, replacing any existing content. It allows for a custom typing speed. The method first checks if there is an active typing coroutine and stops it if necessary to ensure that the new text is set with the animation. Then, it updates the wobble effect indices to encompass the entire new text. Finally, it starts the typewriter routine with the specified text and custom speed in the FindKey game.

    public void SetTextAnimated(string text, float customSpeed) // This method sets the entire text of the story log with a typewriter animation effect, replacing any existing content. It allows for a custom typing speed. The method first checks if there is an active typing coroutine and stops it if necessary to ensure that the new text is set with the animation. Then, it updates the wobble effect indices to encompass the entire new text. Finally, it starts the typewriter routine with the specified text and custom speed in the FindKey game.
    {
        if (!text.Contains("Acción no válida.")) lastLoadedText = text;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        storyText.text = "";

        wobbleStartIndex = 0;
        wobbleEndIndex = -1;

        currentTargetText = text;
        currentCallback = null;
        typingCoroutine = StartCoroutine(TypewriterRoutine(text, customSpeed)); // Start the typewriter routine with the specified text and custom speed, allowing for an animated effect when setting the entire text of the story log in the FindKey game.
    }

    IEnumerator TypewriterRoutine(string lineToAdd, float customSpeed, System.Action onFinished = null) // This coroutine implements the typewriter effect for adding text to the story log. It processes the input text character by character, handling special tags for pauses and ensuring that the typing sound is played appropriately. The coroutine also respects the isSkipping flag, allowing for an immediate display of the full text if skipping is enabled. Once the typing effect is complete, it invokes the optional callback and resets the typing state in the FindKey game.
    {
        isSkipping = false;
        if (storyText.text.Length > 0) storyText.text += "\n";
        bool isInsideTag = false;

        float activeSpeed = customSpeed > 0f ? customSpeed : typingSpeed;
        float currentDelay = activeSpeed / Mathf.Max(0.01f, currentSpeedMultiplier);

        if (storyText.text.EndsWith("_"))
            storyText.text = storyText.text.Substring(0, storyText.text.Length - 1);

        for (int i = 0; i < lineToAdd.Length; i++) // Iterate through each character in the input text, processing it for the typewriter effect. The loop checks for special tags that indicate pauses (e.g., [0.5s]) and handles them accordingly, allowing for dynamic timing in the typing effect. If a pause tag is detected, the coroutine waits for the specified duration before continuing to type the remaining characters. For regular characters, it updates the story text and plays the appropriate typing sound based on the character and the current emotion settings. The loop also checks if skipping is enabled, in which case it will immediately display the full text without waiting for the remaining characters to be typed out in the FindKey game.
        {
            if (lineToAdd[i] == '[')
            {
                int endIndexTag = lineToAdd.IndexOf("s]", i);
                if (endIndexTag != -1)
                {
                    string possibleNumber = lineToAdd.Substring(i + 1, endIndexTag - (i + 1));
                    if (float.TryParse(possibleNumber, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float pauseDuration))
                    {
                        if (!isSkipping && activeSpeed > 0.001f)
                        {
                            yield return new WaitForSeconds(pauseDuration);
                        }

                        i = endIndexTag + 1;
                        continue;
                    }
                }
            }

            char c = lineToAdd[i];
            if (c == '<') isInsideTag = true;

            if (storyText.text.EndsWith("_"))
                storyText.text = storyText.text.Substring(0, storyText.text.Length - 1);

            storyText.text += c;

            if (c == '>') isInsideTag = false;

            if (!isInsideTag) // Only play typing sound and add delay for characters that are not part of a tag, ensuring that the typewriter effect is consistent and that tags are processed correctly without affecting the timing or audio feedback in the FindKey game.
            {
                storyText.text += "_";

                if (!isSkipping)
                {
                    PlayTypingSound(c);
                    UpdateLayout();
                    yield return new WaitForSeconds(currentDelay);
                }
            }
        }

        if (storyText.text.EndsWith("_"))
            storyText.text = storyText.text.Substring(0, storyText.text.Length - 1);

        if (isSkipping) UpdateLayout();

        typingCoroutine = null;
        currentCallback = null;
        onFinished?.Invoke();
    }

    void PlayTypingSound(char c) // This method plays the appropriate typing sound based on the input character and the current emotion settings. It first checks if the character is a whitespace character, in which case it does not play any sound. Then, it checks the specificCharacterSounds list to see if there is a specific sound assigned to the character. If a match is found, it plays that sound. If no specific sound is found, it randomly selects a sound from the narrativeSounds list if available, or falls back to the defaultTypingSound if no narrative sounds are defined. The method also applies random variations to the pitch and volume of the sound for added variety in the audio feedback during the typing effect in the FindKey game.
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

        if (narrativeSounds != null && narrativeSounds.Count > 0) // If there are narrative sounds defined, randomly select one to play for characters that do not have a specific sound assigned. This adds variety to the typing sounds and enhances the auditory experience during the typewriter effect in the FindKey game.
        {
            int randomIndex = Random.Range(0, narrativeSounds.Count);
            PlayCleanTypingSound(narrativeSounds[randomIndex]);
        }

        else if (defaultTypingSound != null && defaultTypingSound.IsValid()) // If no specific sound is found for the character and there are no narrative sounds defined, play the default typing sound if it is valid. This ensures that there is always some audio feedback for typing characters, even if they do not have specific sounds assigned in the FindKey game.
        {
            PlayCleanTypingSound(defaultTypingSound);
        }
    }

    void PlayCleanTypingSound(SoundSettings sound) // This method plays a typing sound based on the provided SoundSettings, applying random variations to the pitch and volume for added variety. It creates a temporary AudioSource based on the typingAudioSource template, sets its properties according to the SoundSettings, and uses an AudioFader component to handle the fade-in and fade-out of the sound. This allows for multiple overlapping typing sounds without interrupting each other, while maintaining consistent audio settings across all typing sounds in the FindKey game.
    {
        float randomPitch = Random.Range(-pitchVariation, pitchVariation);
        float finalPitch = (sound.pitch + randomPitch) * currentPitchMultiplier;

        float randomVol = Random.Range(-volumeVariation, volumeVariation);
        float finalVol = Mathf.Clamp01(sound.volume + randomVol);

        AudioSource tempSource = typingAudioSource.gameObject.AddComponent<AudioSource>();
        tempSource.spatialBlend = typingAudioSource.spatialBlend;
        tempSource.outputAudioMixerGroup = typingAudioSource.outputAudioMixerGroup;
        tempSource.clip = sound.clip;
        tempSource.pitch = finalPitch;
        tempSource.loop = false;

        AudioFader fader = tempSource.gameObject.AddComponent<AudioFader>();
        fader.StartFade(tempSource, finalVol, sound.fadeInDuration, sound.fadeOutDuration, true);
    }

    void UpdateLayout() // This method forces an immediate rebuild of the layout for the contentRect, ensuring that the UI elements are properly updated to accommodate changes in the text. After rebuilding the layout, it starts a coroutine to scroll to the bottom of the scroll view, ensuring that the most recent text is visible in the story log in the FindKey game.
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom() // This coroutine waits until the end of the current frame to ensure that the layout has been updated before setting the vertical normalized position of the scroll view to 0, effectively scrolling to the bottom of the story log. This ensures that the most recent text is visible after any changes to the text content in the FindKey game.
    {
        yield return new WaitForEndOfFrame();
        if (scrollView != null) scrollView.verticalNormalizedPosition = 0f;
    }

    private void OnDisable() // This method is called when the StoryLog component is disabled. It checks if there is an active typing coroutine and stops it if necessary to ensure that any ongoing typewriter effect is halted when the component is no longer active. It also cleans up the text by removing any remaining typewriter tags and invokes the callback if it was set, ensuring that the story log is in a consistent state when re-enabled in the FindKey game.
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
    public void AplicarParametrosATodas() // This method applies the global sound parameters defined in the parametrosGlobales SoundSettings object to all typing sounds used in the StoryLog, including both the default typing sound and any specific character sounds defined in the specificCharacterSounds list. It iterates through each sound and updates its properties (volume, pitch, loop settings, fade durations) to match the global parameters, ensuring consistency across all typing sounds in the FindKey game. The method also counts how many sounds were adjusted and logs a success message with the total count of modified sounds.
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
            foreach (var mapping in specificCharacterSounds) // Iterate through each character sound mapping in the specificCharacterSounds list and apply the global sound parameters to each valid sound. This ensures that all character-specific typing sounds are updated to match the global settings defined in parametrosGlobales, maintaining consistency across the audio design of the typing effect in the FindKey game.
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