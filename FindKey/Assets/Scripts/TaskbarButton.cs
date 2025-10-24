using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class TaskbarButton : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    public Button rootButton;
    private AppWindow linkedWindow;


    public void Setup(AppWindow window, string appName)
    {
        linkedWindow = window;
        if (labelText) labelText.text = appName;
        if (rootButton != null) rootButton.onClick.AddListener(OnClicked);
    }


    private void OnClicked()
    {
        if (linkedWindow == null) return;
        linkedWindow.ToggleMinimizeRestore();
    }
}