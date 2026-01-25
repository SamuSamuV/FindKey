using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AppWindow : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI titleText;
    public Button closeButton;
    public Button minimizeButton;
    public bool isOpen;
    [HideInInspector] public string appName;
    public MoveAppData moveAppData;

    public bool isMinimized = false;

    protected virtual void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (minimizeButton != null) minimizeButton.onClick.AddListener(Minimize);

        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
            moveAppData = goMoveAppData.GetComponent<MoveAppData>();

    }

    public virtual void Setup(string title)
    {
        if (titleText) titleText.text = title;
    }

    public virtual void Close()
    {
        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == appName)
                {
                    data.isOpen = false;
                    data.isMinimized = false;

                    if (data.label == "Move")
                    {
                        foreach (var otherApp in dm.iconsToSpawn)
                        {
                            if (otherApp.label == "Enemy Encounter" && otherApp.windowInstance != null)
                            {
                                EnemyEncounterData enemyScript = otherApp.windowInstance.GetComponent<EnemyEncounterData>();
                                if (enemyScript != null)
                                {
                                    moveAppData.playerIsFrontCat = false;
                                    enemyScript.ResetNPC();
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        TaskbarManager.GetOrFindInstance()?.UnregisterWindow(this);
        isOpen = false;
        Destroy(gameObject);
    }

    public virtual void Minimize()
    {
        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == appName)
                {
                    data.isMinimized = true;
                    data.isOpen = true;
                    break;
                }
            }
        }

        TaskbarManager.GetOrFindInstance()?.UnregisterWindow(this);
        isMinimized = true;
        gameObject.SetActive(false);
    }

    public virtual void Restore()
    {
        if (!isMinimized) return;
        isMinimized = false;
        gameObject.SetActive(true);
        BringToFront();
    }

    public void ToggleMinimizeRestore()
    {
        if (isMinimized) Restore();
        else Minimize();
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BringToFront();
    }
}