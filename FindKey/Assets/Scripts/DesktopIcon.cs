using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(RectTransform))]
public class DesktopIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Image iconImage;
    public TextMeshProUGUI labelText;
    public Image backgroundImage; // fondo del icono (para colorear al seleccionar)
    private RectTransform rt;
    private CanvasGroup cg;
    private Canvas canvas;
    private DesktopManager manager;
    [SerializeField] GameObject desktopIconData;
    private Vector2 originalPos;
    private float clickTime = 0f;
    private const float doubleClickThreshold = 0.4f;


    public Color normalBg = new Color(0, 0, 0, 0); // transparente
    public Color selectedBg = new Color(0.0f, 0.48f, 1f, 0.9f); // azul similar a XP


    public void Setup(string label, Sprite sprite, DesktopManager mgr)
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


    public void SetSelected(bool sel)
    {
        if (backgroundImage) backgroundImage.color = sel ? selectedBg : normalBg;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rt.anchoredPosition;
        if (cg) { cg.blocksRaycasts = false; cg.alpha = 0.9f; }
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 move;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)rt.parent, eventData.position, eventData.pressEventCamera, out move);
        rt.anchoredPosition = move;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (cg) { cg.blocksRaycasts = true; cg.alpha = 1f; }

        Vector2Int grid = manager.PositionToGrid(rt.anchoredPosition);

        if (manager.IsGridOccupied(grid, this))
        {
            rt.anchoredPosition = originalPos;
        }
        else
        {
            rt.anchoredPosition = manager.GridToPosition(grid);
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // single click: select and highlight
        manager.SelectIcon(this);


        // detect double click
        if (Time.time - clickTime < doubleClickThreshold)
        {
            // double click -> launch app
            if (AppLauncher.Instance != null)
            {
                //AppLauncher.Instance.appWindowPrefab = desktopIconData.windowApp;
                AppLauncher.Instance.LaunchApp(labelText.text, rt.anchoredPosition);
            }
        }
        clickTime = Time.time;
    }
}