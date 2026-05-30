using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
/// <summary>
/// Class: FixedVisualHandle
/// Description: This script is designed to be attached to a UI element that serves as a visual handle for a scrollbar in the FindKey game. It allows the handle to be dragged with
///              the mouse, updating the position of the handle and synchronizing it with the value of the associated scrollbar. The script references the original scrollbar and its sliding area
///              (the parent RectTransform) to calculate the appropriate position of the handle based on the scrollbar's value. It implements the IBeginDragHandler, IDragHandler, and IEndDragHandler
///              interfaces to manage the dragging behavior, ensuring that the handle moves smoothly and updates the scrollbar's value accordingly. The LateUpdate method ensures that if
///              the scrollbar's value changes programmatically, the handle's position will update to reflect that change.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class FixedVisualHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias")]
    public Scrollbar nativeScrollbar;
    public RectTransform slidingArea;

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
            UpdatePositionFromValue(nativeScrollbar.value); // Asegure that the handle's position matches the scrollbar's value when not dragging
        }
    }

    private void UpdatePositionFromValue(float val) // This function calculates the handle's position based on the scrollbar's value and updates its anchoredPosition accordingly.
    {
        float totalHeight = slidingArea.rect.height;
        float myHeight = myRect.rect.height;
        float maxTravel = (totalHeight - myHeight) / 2f;

        float newY = Mathf.Lerp(-maxTravel, maxTravel, val);
        myRect.anchoredPosition = new Vector2(0f, newY);
    }

    public void OnBeginDrag(PointerEventData eventData) // This function is called when the user starts dragging the handle. It calculates the offset between the pointer's position and the handle's position to ensure smooth dragging.
    {
        if (slidingArea == null || myRect == null) return;

        isDragging = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(myRect, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }

    public void OnDrag(PointerEventData eventData) // This function is called while the user is dragging the handle. It calculates the new position of the handle based on the pointer's position, clamps it within the sliding area, and updates the scrollbar's value accordingly.
    {
        if (slidingArea == null || nativeScrollbar == null) return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(slidingArea, eventData.position, eventData.pressEventCamera, out localPointerPosition)) // Convert the screen point to a local point within the sliding area
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

    public void OnEndDrag(PointerEventData eventData) // This function is called when the user releases the handle after dragging. It simply sets the isDragging flag to false, allowing the LateUpdate method to update the handle's position based on the scrollbar's value if it changes programmatically.
    {
        isDragging = false;
    }
}