/// <summary>
/// Class: NPCVisualController
/// Description: Control the visual representation of NPCs, managing different animations based on their state and emotion. This includes idle, thinking, and talking animations,
///              with support for blinking and dynamic sprite replacement. 
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCVisualController : MonoBehaviour
{
    public Image npcImage;

    [Header("Configuración General")]
    public float animationSpeed = 0.1f;

    [Header("Animaciones: Idle & Pensando")]
    public Sprite[] idleSprites;
    public Sprite[] idleBlinkSprites;
    public Sprite[] thinkingSprites;

    [Header("Animaciones: Hablando (Neutral)")]
    public Sprite[] neutralTalkSprites;
    public Sprite[] neutralTalkBlinkSprites;

    [Header("Animaciones: Hablando (Happy)")]
    public Sprite[] happyTalkSprites;
    public Sprite[] happyTalkBlinkSprites;

    [Header("Animaciones: Hablando (Sad)")]
    public Sprite[] sadTalkSprites;
    public Sprite[] sadTalkBlinkSprites;

    [Header("Configuración Parpadeo")]
    public float blinkCheckInterval = 3f;
    [Range(0f, 1f)] public float blinkChance = 0.3f;

    public enum NPCState { Idle, Thinking, Talking }
    public enum NPCEmotion { Neutral, Happy, Sad }

    private NPCState currentState = NPCState.Idle;
    private NPCEmotion currentEmotion = NPCEmotion.Neutral;

    private Coroutine animationCoroutine;

    private Sprite[] baseIdle, baseBlink, baseNeutral, baseHappy, baseSad;

    private bool isInitialized = false;

    private void Awake() // Initialize data on Awake to ensure it's ready before any Start or OnEnable calls
    {
        InitializeData();
    }

    public void InitializeData() // Separate method to initialize and store base sprites for easy restoration
    {
        if (isInitialized) return;

        if (npcImage == null) npcImage = GetComponent<Image>();

        baseIdle = idleSprites;
        baseBlink = idleBlinkSprites;
        baseNeutral = neutralTalkSprites;
        baseHappy = happyTalkSprites;
        baseSad = sadTalkSprites;

        isInitialized = true;
    }

    private void Start()
    {

    }

    private void OnEnable() // Start animation when the object is enabled
    {
        ResumeAnimation();
    }

    public void RestoreDefaultSprites() // Method to restore original sprites if they were replaced
    {
        InitializeData();

        idleSprites = baseIdle;
        idleBlinkSprites = baseBlink;
        neutralTalkSprites = baseNeutral;
        happyTalkSprites = baseHappy;
        sadTalkSprites = baseSad;
    }

    public void SetState(NPCState newState, NPCEmotion newEmotion = NPCEmotion.Neutral) // Main method to set the NPC's state and emotion, triggering the appropriate animation
    {
        currentState = newState;
        currentEmotion = newEmotion;

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimationRoutine());
    }

    public void StopAnimation() // Method to stop the current animation, useful for pausing or when the NPC is no longer visible
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
    }

    public void ResumeAnimation() // Method to resume animation, reapplying the current state and emotion to ensure the correct animation plays
    {
        SetState(currentState, currentEmotion);
    }

    public void ReplaceSprites(Sprite[] newIdle, Sprite[] newBlink, Sprite[] newTalk) // Method to replace the current sprites with new ones, allowing for dynamic changes in appearance (e.g., different outfits or expressions)
    {
        if (newIdle != null && newIdle.Length > 0) idleSprites = newIdle;
        if (newBlink != null && newBlink.Length > 0) idleBlinkSprites = newBlink;

        if (newTalk != null && newTalk.Length > 0)
        {
            neutralTalkSprites = newTalk;
            happyTalkSprites = newTalk;
            sadTalkSprites = newTalk;
        }
    }

    private IEnumerator AnimationRoutine() // Coroutine to handle the animation loop, including frame updates and blinking logic based on the current state and emotion
    {
        int frameIndex = 0;
        float timeSinceLastBlink = 0f;

        while (true) // Loop indefinitely, updating the sprite based on the current animation and handling blinking at intervals
        {
            if (npcImage == null) yield break;

            Sprite[] currentAnim = GetCurrentAnimation(out Sprite[] currentBlink);

            if (currentAnim == null || currentAnim.Length == 0)
            {
                yield return null;
                continue;
            }

            timeSinceLastBlink += animationSpeed;

            if (timeSinceLastBlink >= blinkCheckInterval)
            {
                timeSinceLastBlink = 0f;
                if (Random.value <= blinkChance && currentBlink != null && currentBlink.Length > 0)
                {
                    for (int i = 0; i < currentBlink.Length; i++) // Loop through the blink animation frames, displaying each one for the specified animation speed
                    {
                        npcImage.sprite = currentBlink[i];
                        yield return new WaitForSeconds(animationSpeed);
                    }

                    continue;
                }
            }

            if (frameIndex >= currentAnim.Length) frameIndex = 0;
            npcImage.sprite = currentAnim[frameIndex];
            frameIndex++;

            yield return new WaitForSeconds(animationSpeed);
        }
    }

    private Sprite[] GetCurrentAnimation(out Sprite[] blinkAnim) // Method to determine the current animation frames based on the NPC's state and emotion, also providing the appropriate blink animation if available
    {
        blinkAnim = idleBlinkSprites;

        if (currentState == NPCState.Idle) // Idle state prioritizes idle sprites and blinking, with no emotion-based variations
        {
            return idleSprites;
        }

        else if (currentState == NPCState.Thinking) // Thinking state prioritizes thinking sprites, but falls back to idle if not available, and disables blinking since it's a more focused state
        {
            if (thinkingSprites != null && thinkingSprites.Length > 0)
            {
                blinkAnim = null;
                return thinkingSprites;
            }

            return idleSprites;
        }

        else // Talking state
        {
            Sprite[] talkAnim = null;

            if (currentEmotion == NPCEmotion.Happy && happyTalkSprites != null && happyTalkSprites.Length > 0)
            {
                blinkAnim = happyTalkBlinkSprites;
                talkAnim = happyTalkSprites;
            }

            else if (currentEmotion == NPCEmotion.Sad && sadTalkSprites != null && sadTalkSprites.Length > 0)
            {
                blinkAnim = sadTalkBlinkSprites;
                talkAnim = sadTalkSprites;
            }

            else if (neutralTalkSprites != null && neutralTalkSprites.Length > 0)
            {
                blinkAnim = neutralTalkBlinkSprites;
                talkAnim = neutralTalkSprites;
            }

            if (talkAnim == null || talkAnim.Length == 0)
            {
                blinkAnim = idleBlinkSprites;
                return idleSprites;
            }

            return talkAnim;
        }
    }
}