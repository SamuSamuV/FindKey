using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// Class: CustomCursorManager
/// Description: This class manages the custom cursor behavior in the game, including changing the cursor sprite based on interactions and handling a loading animation.
///              It uses Unity's new Input System to track mouse position and clicks, and it checks for UI interactions to update the cursor accordingly.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class CustomCursorManager : MonoBehaviour
{
    public static CustomCursorManager Instance { get; private set; } // Singleton to globally access the cursor manager

    [Header("Referencias UI")]
    public Image cursorImage;
    public Canvas cursorCanvas;

    [Header("Sprites Estáticos")]
    public Sprite normalCursor;
    public Sprite clickCursor;

    [Header("Animación de Carga (Loading)")]
    public Sprite[] loadingCursorFrames;
    public float loadingAnimationSpeed = 0.1f;

    [Header("Ajustes")]
    public Vector2 hotspotOffset = Vector2.zero;

    [HideInInspector] public bool isLoading = false;

    private int currentLoadingFrame = 0;
    private float loadingAnimationTimer = 0f;

    private void Awake() // Ensure singleton pattern
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() // Hide the default system cursor
    {
        Cursor.visible = false;
    }

    private void Update() // Main update loop to handle cursor behavior
    {
        if (Mouse.current == null) return;

        MoveCursor(); // Update cursor position to follow the mouse

        if (isLoading && loadingCursorFrames != null && loadingCursorFrames.Length > 0)
        {
            loadingAnimationTimer += Time.deltaTime;

            if (loadingAnimationTimer >= loadingAnimationSpeed)
            {
                loadingAnimationTimer = 0f;
                currentLoadingFrame++;

                if (currentLoadingFrame >= loadingCursorFrames.Length)
                {
                    currentLoadingFrame = 0;
                }
            }

            cursorImage.sprite = loadingCursorFrames[currentLoadingFrame];
        }

        else if (IsHoveringInteractable()) // Change cursor to click sprite when hovering over interactable UI elements
        {
            cursorImage.sprite = clickCursor;
        }

        else
        {
            cursorImage.sprite = normalCursor;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.Play("click");
            }
        }
    }

    private void MoveCursor() // Update the cursor image position to follow the mouse, applying the hotspot offset
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        cursorImage.transform.position = mousePos + hotspotOffset;
    }

    private bool IsHoveringInteractable() // Check if the mouse is currently hovering over any interactable UI elements (buttons, toggles, desktop icons, taskbar buttons, input fields)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = mousePos };
        List<RaycastResult> results = new List<RaycastResult>();

        if (EventSystem.current != null) EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results) // Check each raycast result for interactable components
        {
            GameObject obj = result.gameObject;
            if (obj.GetComponent<Button>() != null) return true;
            if (obj.GetComponent<Toggle>() != null) return true;
            if (obj.GetComponent<DesktopIcon>() != null || obj.GetComponentInParent<DesktopIcon>() != null) return true;
            if (obj.GetComponent<TaskbarButton>() != null || obj.GetComponentInParent<TaskbarButton>() != null) return true;
            if (obj.GetComponent<TMPro.TMP_InputField>() != null) return true;
        }

        return false;
    }
}