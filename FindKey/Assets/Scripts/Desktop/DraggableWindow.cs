using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform windowRoot;
    private Vector2 offset;
    private Canvas canvas;
    private AppWindow appWindow;

    private void Awake()
    {
        if (!windowRoot) windowRoot = GetComponentInParent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        appWindow = GetComponent<AppWindow>();
        if (appWindow == null) appWindow = GetComponentInParent<AppWindow>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(windowRoot, eventData.position, eventData.pressEventCamera, out offset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null || windowRoot == null)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)windowRoot.parent,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        windowRoot.anchoredPosition = pos - offset;
        ClampToCanvas();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ClampToCanvas();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (appWindow != null)
        {
            appWindow.BringToFront();
        }
    }

    private void ClampToCanvas()
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
