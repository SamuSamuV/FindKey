/// <summary>
/// Class: DesktopAreaClick
/// Description: This script handles click events on the desktop area in the FindKey game. When the user clicks on the desktop, it deselects any currently selected icons and closes
///              any open menus from the taskbar. The script implements the IPointerClickHandler interface to detect pointer click events and interacts with the DesktopManager to manage icon
///              selection and with the TaskbarManager to manage open menus. This ensures a smooth user experience when interacting with the desktop area, allowing for easy deselection of icons
///              and closing of menus with a simple click.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopAreaClick : MonoBehaviour, IPointerClickHandler
{
    public DesktopManager manager;

    public void OnPointerClick(PointerEventData eventData) // This method is called when the desktop area is clicked
    {
        manager?.DeselectIcon();

        TaskbarManager.GetOrFindInstance()?.CloseAllMenus();
    }
}