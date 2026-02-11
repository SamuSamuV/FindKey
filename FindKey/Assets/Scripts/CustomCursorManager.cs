using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem; // <--- NECESARIO PARA EL NUEVO SISTEMA

public class CustomCursorManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image cursorImage;
    public Canvas cursorCanvas;

    [Header("Sprites")]
    public Sprite normalCursor;
    public Sprite clickCursor;

    [Header("Ajustes")]
    public Vector2 hotspotOffset = Vector2.zero;

    private void Start()
    {
        // Ocultar el cursor nativo del sistema
        Cursor.visible = false;
    }

    private void Update()
    {
        // Verificamos que exista un ratón conectado para evitar errores
        if (Mouse.current == null) return;

        // 1. Mover la imagen a la posición del ratón
        MoveCursor();

        // 2. Comprobar si hay algo clicable debajo
        if (IsHoveringInteractable())
        {
            cursorImage.sprite = clickCursor;
        }
        else
        {
            cursorImage.sprite = normalCursor;
        }
    }

    private void MoveCursor()
    {
        // --- CAMBIO PARA EL NUEVO INPUT SYSTEM ---
        Vector2 mousePos = Mouse.current.position.ReadValue();

        cursorImage.transform.position = mousePos + hotspotOffset;
    }

    private bool IsHoveringInteractable()
    {
        // --- CAMBIO PARA EL NUEVO INPUT SYSTEM ---
        Vector2 mousePos = Mouse.current.position.ReadValue();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = mousePos
        };

        List<RaycastResult> results = new List<RaycastResult>();

        // Lanzamos el Raycast UI
        if (EventSystem.current != null)
        {
            EventSystem.current.RaycastAll(pointerData, results);
        }

        foreach (RaycastResult result in results)
        {
            GameObject obj = result.gameObject;

            // 1. Botones estándar de Unity (UI Button)
            if (obj.GetComponent<Button>() != null) return true;

            // 2. Toggles 
            if (obj.GetComponent<Toggle>() != null) return true;

            // 3. Tus Iconos del Escritorio
            if (obj.GetComponent<DesktopIcon>() != null || obj.GetComponentInParent<DesktopIcon>() != null) return true;

            // 4. Tus Botones de la Barra de Tareas
            if (obj.GetComponent<TaskbarButton>() != null || obj.GetComponentInParent<TaskbarButton>() != null) return true;

            // 5. Entradas de texto (InputFields)
            if (obj.GetComponent<TMPro.TMP_InputField>() != null) return true;
        }

        return false;
    }
}