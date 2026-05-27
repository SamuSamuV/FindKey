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

    private void Awake()
    {
        InitializeData();
    }

    // Esta función guarda los sprites del Inspector a salvo.
    public void InitializeData()
    {
        if (isInitialized) return; // Si ya se hizo, no lo repetimos

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
        // Lo dejamos vacío porque OnEnable ya arranca la animación automáticamente.
        // Si forzamos el Idle aquí, podríamos pisar a la IA si ya ha empezado a pensar.
    }

    private void OnEnable()
    {
        ResumeAnimation();
    }

    public void RestoreDefaultSprites()
    {
        InitializeData(); // CLAVE: Si la IA llama a esto estando apagado, se autoinicializa primero.

        idleSprites = baseIdle;
        idleBlinkSprites = baseBlink;
        neutralTalkSprites = baseNeutral;
        happyTalkSprites = baseHappy;
        sadTalkSprites = baseSad;
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

    private IEnumerator AnimationRoutine()
    {
        int frameIndex = 0;
        float timeSinceLastBlink = 0f;

        while (true)
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
        // Por defecto, si algo falla, preparamos el parpadeo del Idle
        blinkAnim = idleBlinkSprites;

        if (currentState == NPCState.Idle)
        {
            return idleSprites;
        }
        else if (currentState == NPCState.Thinking)
        {
            // Si tiene sprites de pensar, los usa
            if (thinkingSprites != null && thinkingSprites.Length > 0)
            {
                blinkAnim = null; // No parpadea mientras piensa
                return thinkingSprites;
            }
            // FALLBACK: Si no tiene sprites de pensar, sigue haciendo el Idle normal
            return idleSprites;
        }
        else // ESTADO TALKING
        {
            Sprite[] talkAnim = null;

            // Busca la emoción correcta asegurándose de que la lista NO esté vacía
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

            // FALLBACK ABSOLUTO: Si la IA manda hablar pero no has puesto ningún sprite de hablar en el Inspector...
            if (talkAnim == null || talkAnim.Length == 0)
            {
                blinkAnim = idleBlinkSprites;
                return idleSprites; // Se queda haciendo el Idle sin romperse
            }

            return talkAnim;
        }
    }
}