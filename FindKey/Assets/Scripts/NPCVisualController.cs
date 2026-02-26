using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCVisualController : MonoBehaviour
{
    public Image npcImage;

    [Header("Sprites de Estado")]
    public Sprite idleSprite;
    public Sprite thinkingSprite;
    public Sprite blinkSprite;

    [Header("Animación Hablando")]
    public Sprite[] talkingSprites;
    public float talkAnimationSpeed = 0.15f;

    [Header("Configuración Parpadeo")]
    public float blinkDuration = 0.15f;
    public float blinkCheckInterval = 3f;
    [Range(0f, 1f)] public float blinkChance = 0.2f;

    public enum NPCState { Idle, Thinking, Talking }
    private NPCState currentState = NPCState.Idle;

    private Coroutine talkCoroutine;
    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (npcImage == null) npcImage = GetComponent<Image>();
        SetState(NPCState.Idle);
    }

    public void SetState(NPCState newState)
    {
        currentState = newState;

        if (talkCoroutine != null) StopCoroutine(talkCoroutine);
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        switch (newState)
        {
            case NPCState.Idle:
                if (idleSprite) npcImage.sprite = idleSprite;
                blinkCoroutine = StartCoroutine(IdleBlinkRoutine());
                break;

            case NPCState.Thinking:
                if (thinkingSprite) npcImage.sprite = thinkingSprite;
                break;

            case NPCState.Talking:
                if (talkingSprites != null && talkingSprites.Length > 0)
                {
                    talkCoroutine = StartCoroutine(TalkRoutine());
                }

                else
                {
                    if (idleSprite) npcImage.sprite = idleSprite;
                }

                break;
        }
    }

    private IEnumerator TalkRoutine()
    {
        int index = 0;
        while (true)
        {
            npcImage.sprite = talkingSprites[index];
            index++;
            if (index >= talkingSprites.Length) index = 0;

            yield return new WaitForSeconds(talkAnimationSpeed);
        }
    }

    private IEnumerator IdleBlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkCheckInterval);

            if (Random.value <= blinkChance && blinkSprite != null)
            {
                npcImage.sprite = blinkSprite;
                yield return new WaitForSeconds(blinkDuration);

                npcImage.sprite = idleSprite;
            }
        }
    }
}