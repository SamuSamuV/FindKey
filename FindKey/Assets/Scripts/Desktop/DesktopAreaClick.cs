using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopAreaClick : MonoBehaviour, IPointerClickHandler
{
    public DesktopManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        manager?.DeselectIcon();
    }
}