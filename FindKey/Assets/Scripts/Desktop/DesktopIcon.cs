/// <summary>
/// Class: DesktopIcon
/// Description: Represents a draggable desktop icon; handles selection, dragging, double-click launching and grid snapping.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]

public class DesktopIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Image iconImage;
    public TextMeshProUGUI labelText;
    public Image backgroundImage; // icon background (used to color on selection)
    private RectTransform rt;
    private CanvasGroup cg;
    private Canvas canvas;
    private DesktopManager manager;
    [SerializeField] GameObject desktopIconData;
    private Vector2 originalPos;
    private float clickTime = 0f;
    private const float doubleClickThreshold = 0.4f;


    public Color normalBg = new Color(0, 0, 0, 0); // transparent
    public Color selectedBg = new Color(0.0f, 0.48f, 1f, 0.9f); // blue similar to XP


    public void Setup(string label, Sprite sprite, DesktopManager mgr) // called by DesktopManager when spawning the icon
    {
        labelText.text = label;
        if (sprite != null) iconImage.sprite = sprite;
        manager = mgr;
    }

    void Start()
    {

    }


    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        if (backgroundImage) backgroundImage.color = normalBg;
    }


    public void SetSelected(bool sel) // called by DesktopManager when selecting/deselecting the icon
    {
        if (backgroundImage) backgroundImage.color = sel ? selectedBg : normalBg;
    }


    public void OnBeginDrag(PointerEventData eventData) // called by the EventSystem when starting to drag the icon
    {
        originalPos = rt.anchoredPosition;
        if (cg) { cg.blocksRaycasts = false; cg.alpha = 0.9f; }
    }


    public void OnDrag(PointerEventData eventData) // called by the EventSystem while dragging the icon
    {
        Vector2 move;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)rt.parent, eventData.position, eventData.pressEventCamera, out move);
        rt.anchoredPosition = move;
    }


    public void OnEndDrag(PointerEventData eventData) // called by the EventSystem when releasing the icon after dragging
    {
        if (cg) { cg.blocksRaycasts = true; cg.alpha = 1f; }

        Vector2Int grid = manager.PositionToGrid(rt.anchoredPosition);

        Vector2 targetPos;

        if (manager.IsGridOccupied(grid, this)) // If the grid is occupied, snap back to original position
        {
            targetPos = originalPos;
        }

        else
        {
            targetPos = manager.GridToPosition(grid); // Snap to the grid position
        }

        rt.anchoredPosition = ClampToCanvas(targetPos);
    }


    public void OnPointerClick(PointerEventData eventData) // called by the EventSystem when clicking on the icon
    {
        // Single click - visual selection
        manager.SelectIcon(this);

        // Detect double click
        if (Time.time - clickTime < doubleClickThreshold)
        {
            // Double click - launch individual app
            if (AppLauncher.Instance != null)
            {
                // Find if this icon has a window prefab assigned from DesktopManager
                DesktopManager dm = manager;

                // Search in iconsToSpawn list for one matching this label
                foreach (var data in dm.iconsToSpawn)
                {
                    if (data.label == labelText.text && data.windowApp != null)
                    {
                        AppLauncher.Instance.appWindowPrefab = data.windowApp; // assigns the individual app
                        break;
                    }
                }

                // Launch the app normally
                AppLauncher.Instance.LaunchApp(labelText.text, rt.anchoredPosition);
            }
        }

        clickTime = Time.time;
    }

    private Vector2 ClampToCanvas(Vector2 pos) // Ensures the icon stays within the bounds of the canvas when dragged
    {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        RectTransform iconRT = rt;

        // Sizes
        Vector2 canvasSize = canvasRT.rect.size;
        Vector2 iconSize = iconRT.rect.size;

        // Limits (centered on the canvas anchor)
        float minX = -canvasSize.x * 0.5f + iconSize.x * 0.5f;
        float maxX = canvasSize.x * 0.5f - iconSize.x * 0.5f;

        float minY = -canvasSize.y * 0.5f + iconSize.y * 0.5f;
        float maxY = canvasSize.y * 0.5f - iconSize.y * 0.5f;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        return pos;
    }
}