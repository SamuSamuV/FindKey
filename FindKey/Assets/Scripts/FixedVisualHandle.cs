using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
// Hemos añadido los Handlers para poder arrastrar la imagen con el ratón
public class FixedVisualHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias")]
    public Scrollbar nativeScrollbar; // La barra original
    public RectTransform slidingArea; // El Sliding Area (padre)

    private RectTransform myRect;
    private Vector2 pointerOffset;
    private bool isDragging = false;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (!isDragging && nativeScrollbar != null && slidingArea != null && myRect != null)
        {
            UpdatePositionFromValue(nativeScrollbar.value);
        }
    }

    private void UpdatePositionFromValue(float val)
    {
        float totalHeight = slidingArea.rect.height;
        float myHeight = myRect.rect.height;
        float maxTravel = (totalHeight - myHeight) / 2f;

        float newY = Mathf.Lerp(-maxTravel, maxTravel, val);
        myRect.anchoredPosition = new Vector2(0f, newY);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slidingArea == null || myRect == null) return;

        isDragging = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(myRect, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slidingArea == null || nativeScrollbar == null) return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(slidingArea, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            float targetY = localPointerPosition.y - pointerOffset.y;

            float totalHeight = slidingArea.rect.height;
            float myHeight = myRect.rect.height;
            float maxTravel = (totalHeight - myHeight) / 2f;

            float clampedY = Mathf.Clamp(targetY, -maxTravel, maxTravel);

            myRect.anchoredPosition = new Vector2(0f, clampedY);

            float newValue = Mathf.InverseLerp(-maxTravel, maxTravel, clampedY);
            nativeScrollbar.value = newValue;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}