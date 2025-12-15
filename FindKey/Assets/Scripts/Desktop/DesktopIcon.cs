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

        Vector2 targetPos;

        if (manager.IsGridOccupied(grid, this))
        {
            targetPos = originalPos;
        }
        else
        {
            targetPos = manager.GridToPosition(grid);
        }

        rt.anchoredPosition = ClampToCanvas(targetPos);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // Single click  selección visual
        manager.SelectIcon(this);

        // Detectar doble clic
        if (Time.time - clickTime < doubleClickThreshold)
        {
            // Doble clic  lanzar app individual
            if (AppLauncher.Instance != null)
            {
                // Buscar si este icono tiene un prefab de ventana asignado desde DesktopManager
                DesktopManager dm = manager;

                // Buscamos en la lista de iconosToSpawn cuál coincide con este label
                foreach (var data in dm.iconsToSpawn)
                {
                    if (data.label == labelText.text && data.windowApp != null)
                    {
                        AppLauncher.Instance.appWindowPrefab = data.windowApp; //asigna la app individual
                        break;
                    }
                }

                // Lanza la app normalmente
                AppLauncher.Instance.LaunchApp(labelText.text, rt.anchoredPosition);
            }
        }

        clickTime = Time.time;
    }

    private Vector2 ClampToCanvas(Vector2 pos)
    {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        RectTransform iconRT = rt;

        // Tamaños
        Vector2 canvasSize = canvasRT.rect.size;
        Vector2 iconSize = iconRT.rect.size;

        // Límites (centrados en anchor del canvas)
        float minX = -canvasSize.x * 0.5f + iconSize.x * 0.5f;
        float maxX = canvasSize.x * 0.5f - iconSize.x * 0.5f;

        float minY = -canvasSize.y * 0.5f + iconSize.y * 0.5f;
        float maxY = canvasSize.y * 0.5f - iconSize.y * 0.5f;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        return pos;
    }
}