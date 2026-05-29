using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))] // Ahora pide un componente Image, no un SpriteRenderer

public class BackgroundAnim : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [SerializeField] private Sprite[] animationSprites;
    [SerializeField] private float timePerFrame = 0.1f;
    [SerializeField] private bool loop = true;

    private Image imageComponent; // Cambiamos el tipo a Image
    private int currentFrameIndex = 0;
    private float timer = 0f;

    private void Awake()
    {
        // Buscamos el componente Image en este objeto
        imageComponent = GetComponent<Image>();
    }

    private void Start()
    {
        if (imageComponent != null && animationSprites.Length > 0)
        {
            imageComponent.sprite = animationSprites[0];
        }
    }

    private void Update()
    {
        if (imageComponent == null || animationSprites == null || animationSprites.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= timePerFrame)
        {
            timer -= timePerFrame;
            AdvanceFrame();
        }
    }

    private void AdvanceFrame()
    {
        currentFrameIndex++;

        if (currentFrameIndex >= animationSprites.Length)
        {
            if (loop) currentFrameIndex = 0;
            else { currentFrameIndex = animationSprites.Length - 1; enabled = false; return; }
        }

        // Asignamos el sprite al componente Image
        imageComponent.sprite = animationSprites[currentFrameIndex];
    }
}