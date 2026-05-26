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

    private void Awake()
    {
        baseIdle = idleSprites;
        baseBlink = idleBlinkSprites;
        baseNeutral = neutralTalkSprites;
        baseHappy = happyTalkSprites;
        baseSad = sadTalkSprites;
    }

    private void Start()
    {
        if (npcImage == null) npcImage = GetComponent<Image>();
        SetState(NPCState.Idle);
    }

    public void SetState(NPCState newState, NPCEmotion newEmotion = NPCEmotion.Neutral)
    {
        currentState = newState;
        currentEmotion = newEmotion;

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimationRoutine());
    }

    public void StopAnimation()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
    }

    public void ResumeAnimation()
    {
        SetState(currentState, currentEmotion);
    }

    public void ReplaceSprites(Sprite[] newIdle, Sprite[] newBlink, Sprite[] newTalk)
    {
        if (newIdle != null && newIdle.Length > 0) idleSprites = newIdle;
        if (newBlink != null && newBlink.Length > 0) idleBlinkSprites = newBlink;

        // Reemplazamos todos los estados de habla por los del gato corrupto
        if (newTalk != null && newTalk.Length > 0)
        {
            neutralTalkSprites = newTalk;
            happyTalkSprites = newTalk;
            sadTalkSprites = newTalk;
        }
    }

    public void RestoreDefaultSprites()
    {
        idleSprites = baseIdle;
        idleBlinkSprites = baseBlink;
        neutralTalkSprites = baseNeutral;
        happyTalkSprites = baseHappy;
        sadTalkSprites = baseSad;
    }
    // ------------------------------------------------------------------

    private IEnumerator AnimationRoutine()
    {
        int frameIndex = 0;
        float timeSinceLastBlink = 0f;

        while (true)
        {
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
                    for (int i = 0; i < currentBlink.Length; i++)
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

    private Sprite[] GetCurrentAnimation(out Sprite[] blinkAnim)
    {
        blinkAnim = null;

        if (currentState == NPCState.Idle)
        {
            blinkAnim = idleBlinkSprites;
            return idleSprites;
        }
        else if (currentState == NPCState.Thinking)
        {
            return thinkingSprites;
        }
        else
        {
            switch (currentEmotion)
            {
                case NPCEmotion.Happy:
                    blinkAnim = happyTalkBlinkSprites;
                    return happyTalkSprites;
                case NPCEmotion.Sad:
                    blinkAnim = sadTalkBlinkSprites;
                    return sadTalkSprites;
                case NPCEmotion.Neutral:
                default:
                    blinkAnim = neutralTalkBlinkSprites;
                    return neutralTalkSprites;
            }
        }
    }
}