using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class: DraggableWindow
/// Description: This script allows a UI window to be draggable within the bounds of its parent canvas in the FindKey game. It implements the necessary interfaces to handle drag
///              events and pointer interactions. The script calculates the offset between the mouse position and the window's position when dragging starts, and updates the window's position
///              accordingly during the drag. It also ensures that the window stays within the bounds of the canvas. Additionally, it brings the window to the front when it is clicked or dragged,
///              ensuring that it is not obscured by other UI elements.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public RectTransform windowRoot;
    private Vector2 offset;
    private Canvas canvas;
    private AppWindow appWindow;

    private void Awake()
    {
        if (!windowRoot) windowRoot = GetComponentInParent<RectTransform>(); // Try to find the RectTransform if not assigned
        canvas = GetComponentInParent<Canvas>();

        appWindow = GetComponent<AppWindow>();
        if (appWindow == null) appWindow = GetComponentInParent<AppWindow>(); // Try to find AppWindow in parent if not on the same GameObject
    }

    public void OnBeginDrag(PointerEventData eventData) // This method is called when the user starts dragging the window
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(windowRoot, eventData.position, eventData.pressEventCamera, out offset);

        if (appWindow != null)
        {
            appWindow.BringToFront(); // Ensure the window is brought to the front when dragging starts
        }
    }

    public void OnDrag(PointerEventData eventData) // This method is called while the user is dragging the window
    {
        if (canvas == null || windowRoot == null)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)windowRoot.parent,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        ); // Convert the screen point to a local point in the parent RectTransform

        windowRoot.anchoredPosition = pos - offset; // Update the window's anchored position based on the mouse position and the initial offset
        ClampToCanvas(); // Ensure the window stays within the bounds of the canvas while dragging
    }

    public void OnEndDrag(PointerEventData eventData) // This method is called when the user stops dragging the window
    {
        ClampToCanvas();
    }

    public void OnPointerDown(PointerEventData eventData) // This method is called when the user clicks on the window
    {
        if (appWindow != null)
        {
            appWindow.BringToFront();
        }
    }

    private void ClampToCanvas() // This method ensures that the window stays within the bounds of the canvas
    {
        if (canvas == null || windowRoot == null)
            return;

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 windowSize = windowRoot.rect.size;

        float minX = -canvasRect.rect.width / 2 + windowSize.x / 2;
        float maxX = canvasRect.rect.width / 2 - windowSize.x / 2;
        float minY = -canvasRect.rect.height / 2 + windowSize.y / 2;
        float maxY = canvasRect.rect.height / 2 - windowSize.y / 2;

        Vector2 clamped = windowRoot.anchoredPosition;
        clamped.x = Mathf.Clamp(clamped.x, minX, maxX);
        clamped.y = Mathf.Clamp(clamped.y, minY, maxY);

        windowRoot.anchoredPosition = clamped;
    }
}