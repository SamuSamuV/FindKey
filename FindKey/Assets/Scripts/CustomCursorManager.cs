using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        MoveCursor();

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
        Vector2 mousePos = Mouse.current.position.ReadValue();

        cursorImage.transform.position = mousePos + hotspotOffset;
    }

    private bool IsHoveringInteractable()
    {
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

            if (obj.GetComponent<Button>() != null) return true;

            if (obj.GetComponent<Toggle>() != null) return true;

            if (obj.GetComponent<DesktopIcon>() != null || obj.GetComponentInParent<DesktopIcon>() != null) return true;

            if (obj.GetComponent<TaskbarButton>() != null || obj.GetComponentInParent<TaskbarButton>() != null) return true;

            if (obj.GetComponent<TMPro.TMP_InputField>() != null) return true;
        }

        return false;
    }
}