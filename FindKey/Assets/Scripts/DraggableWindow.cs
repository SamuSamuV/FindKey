using UnityEngine;
using UnityEngine.EventSystems;


public class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform windowRoot;
    private Vector2 offset;
    private Canvas canvas;


    private void Awake()
    {
        if (!windowRoot) windowRoot = GetComponentInParent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(windowRoot, eventData.position, eventData.pressEventCamera, out offset);
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)windowRoot.parent, eventData.position, eventData.pressEventCamera, out pos);
        windowRoot.anchoredPosition = pos - offset;
    }


    public void OnEndDrag(PointerEventData eventData) { }
}