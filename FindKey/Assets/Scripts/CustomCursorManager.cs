using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CustomCursorManager : MonoBehaviour
{
    public static CustomCursorManager Instance { get; private set; }

    [Header("Referencias UI")]
    public Image cursorImage;
    public Canvas cursorCanvas;

    [Header("Sprites Estáticos")]
    public Sprite normalCursor;
    public Sprite clickCursor;

    [Header("Animación de Carga (Loading)")]
    public Sprite[] loadingCursorFrames; // Lista de imágenes para la animación
    public float loadingAnimationSpeed = 0.1f; // Velocidad a la que cambian los frames

    [Header("Ajustes")]
    public Vector2 hotspotOffset = Vector2.zero;

    [HideInInspector] public bool isLoading = false;

    // Variables internas para controlar la animación
    private int currentLoadingFrame = 0;
    private float loadingAnimationTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        MoveCursor();

        // LÓGICA DE ANIMACIÓN DE CARGA
        if (isLoading && loadingCursorFrames != null && loadingCursorFrames.Length > 0)
        {
            // Sumamos el tiempo que ha pasado
            loadingAnimationTimer += Time.deltaTime;

            // Si supera la velocidad asignada, pasamos al siguiente frame
            if (loadingAnimationTimer >= loadingAnimationSpeed)
            {
                loadingAnimationTimer = 0f; // Reseteamos temporizador
                currentLoadingFrame++;      // Avanzamos de imagen

                // Si llegamos al final de la lista, volvemos a empezar (Bucle)
                if (currentLoadingFrame >= loadingCursorFrames.Length)
                {
                    currentLoadingFrame = 0;
                }
            }

            // Aplicamos la imagen actual de la animación
            cursorImage.sprite = loadingCursorFrames[currentLoadingFrame];
        }
        else if (IsHoveringInteractable())
        {
            // Si no está cargando pero pasamos por encima de un botón
            cursorImage.sprite = clickCursor;
        }
        else
        {
            // Cursor normal por defecto
            cursorImage.sprite = normalCursor;
        }

        // Sonido de clic general
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.Play("click");
            }
        }
    }

    private void MoveCursor()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        cursorImage.transform.position = mousePos + hotspotOffset;
    }

    private bool IsHoveringInteractable()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = mousePos };
        List<RaycastResult> results = new List<RaycastResult>();

        if (EventSystem.current != null) EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
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