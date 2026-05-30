/// <summary>
/// Class: BackgroundAnim
/// Description: This script manages the animation of a background image in the FindKey game. It allows for cycling through a series of sprites to create an animated effect.
///              The script references an Image component to display the sprites and an array of sprites that represent the frames of the animation. The animation can be configured to
///              loop or play once, and the time between frames can be adjusted. The script updates the displayed sprite based on a timer, creating a smooth animation effect for the background.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))] // Now requires an Image component, not a SpriteRenderer

public class BackgroundAnim : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [SerializeField] private Sprite[] animationSprites;
    [SerializeField] private float timePerFrame = 0.1f;
    [SerializeField] private bool loop = true;

    private Image imageComponent; // Changed the type to Image
    private int currentFrameIndex = 0;
    private float timer = 0f;

    private void Awake()
    {
        // Find the Image component on this object
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

    private void AdvanceFrame() // This function advances to the next frame of the animation
    {
        currentFrameIndex++;

        if (currentFrameIndex >= animationSprites.Length)
        {
            if (loop) currentFrameIndex = 0;
            else { currentFrameIndex = animationSprites.Length - 1; enabled = false; return; }
        }

        // Assign the sprite to the Image component
        imageComponent.sprite = animationSprites[currentFrameIndex];
    }
}