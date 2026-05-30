/// <summary>
/// Class: TaskbarButton
/// Description: This script manages the behavior of a taskbar button in the FindKey game. Each taskbar button is linked to an AppWindow and allows the player to minimize or restore
///              the window by clicking on the button. The script references a TextMeshProUGUI component for displaying the app name, an Image component for showing the app icon, and a Button
///              component for handling click interactions. When the button is clicked, it toggles the minimize/restore state of the linked AppWindow, providing a way for players to manage their
///              open windows through the taskbar interface.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskbarButton : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    public Image iconImage;
    public Button rootButton;
    private AppWindow linkedWindow;

    public void Setup(AppWindow window, string appName, Sprite iconSprite) // Call this method to initialize the taskbar button with the linked AppWindow, app name, and icon sprite
    {
        linkedWindow = window;
        if (labelText) labelText.text = appName;
        if (iconImage && iconSprite != null) iconImage.sprite = iconSprite;

        if (rootButton != null)
        {
            rootButton.onClick.RemoveAllListeners();
            rootButton.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked() // This method is called when the taskbar button is clicked. It toggles the minimize/restore state of the linked AppWindow.
    {
        if (linkedWindow == null) return;

        linkedWindow.ToggleMinimizeRestore();
    }
}