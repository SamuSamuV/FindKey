using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskbarButton : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    public Image iconImage;
    public Button rootButton;
    private AppWindow linkedWindow;

    public void Setup(AppWindow window, string appName, Sprite iconSprite)
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

    private void OnClicked()
    {
        if (linkedWindow == null) return;

        linkedWindow.ToggleMinimizeRestore();
    }
}