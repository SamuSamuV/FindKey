using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DesktopManager : MonoBehaviour
{
    public RectTransform desktopArea; // the UI area considered "desktop"
    public GameObject iconPrefab; // prefab representing an icon (image + label)
    public Vector2Int gridSize = new Vector2Int(19, 11); // 19x11 grid
    public Vector2 iconCellSize = new Vector2(64, 64); // adjust to your UI scale
    public Vector2 iconPadding = new Vector2(8, 24);


    [HideInInspector]
    public List<GameObject> icons = new List<GameObject>();


    [HideInInspector]
    public DesktopIcon selectedIcon;


    private void Start()
    {
        // create single icon "game.exe"
        SpawnIcon(new Vector2Int(0, 0), "game.exe", null);
    }


    public void SpawnIcon(Vector2Int gridPos, string label, Sprite sprite)
    {
        if (!iconPrefab) return;
        GameObject go = Instantiate(iconPrefab, desktopArea);
        DesktopIcon di = go.GetComponent<DesktopIcon>();
        di.Setup(label, sprite, this);
        RectTransform rt = go.GetComponent<RectTransform>();
        Vector2 pos = GridToPosition(gridPos);
        rt.anchoredPosition = pos;
        icons.Add(go);
    }


    public Vector2 GridToPosition(Vector2Int gridPos)
    {
        float width = desktopArea.rect.width;
        float height = desktopArea.rect.height;
        float cellW = width / gridSize.x;
        float cellH = height / gridSize.y;
        float x = (cellW * gridPos.x) + cellW / 2 - width / 2;
        float y = height / 2 - (cellH * gridPos.y) - cellH / 2;
        return new Vector2(x, y);
    }


    public Vector2Int PositionToGrid(Vector2 anchoredPos)
    {
        float width = desktopArea.rect.width;
        float height = desktopArea.rect.height;
        float cellW = width / gridSize.x;
        float cellH = height / gridSize.y;


        float localX = anchoredPos.x + width / 2;
        float localY = height / 2 - anchoredPos.y;


        int gx = Mathf.Clamp(Mathf.FloorToInt(localX / cellW), 0, gridSize.x - 1);
        int gy = Mathf.Clamp(Mathf.FloorToInt(localY / cellH), 0, gridSize.y - 1);
        return new Vector2Int(gx, gy);
    }


    // Deselect current icon
    public void DeselectIcon()
    {
        if (selectedIcon != null) selectedIcon.SetSelected(false);
        selectedIcon = null;
    }


    // Select an icon (used by DesktopIcon)
    public void SelectIcon(DesktopIcon icon)
    {
        if (selectedIcon == icon) return;
        DeselectIcon();
        selectedIcon = icon;
        if (selectedIcon != null) selectedIcon.SetSelected(true);
    }
}