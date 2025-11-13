using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesktopManager : MonoBehaviour
{
    [Header("Desktop Settings")]
    public RectTransform desktopArea;
    public GameObject iconPrefab;
    public Vector2 iconCellSize = new Vector2(80, 80);
    public Vector2 iconPadding = new Vector2(10, 20); // espacio entre iconos

    [Header("Icon Data")]
    public DesktopIconData[] iconsToSpawn;

    [HideInInspector] public List<GameObject> icons = new List<GameObject>();
    [HideInInspector] public DesktopIcon selectedIcon;

    private int nextColumn = 0;
    private int nextRow = 0;

    private void Start()
    {
        StartCoroutine(SpawnIconsAfterLayout());
    }

    private IEnumerator SpawnIconsAfterLayout()
    {
        yield return null;

        foreach (var data in iconsToSpawn)
        {
            if (data == null) continue;

            Vector2Int pos = data.gridPos;

            if (pos.x < 0 || pos.y < 0)
                pos = GetNextAutoGridPosition();

            SpawnIcon(pos, data.label, data.sprite);
        }
    }

    private Vector2Int GetNextAutoGridPosition()
    {
        Vector2Int pos = new Vector2Int(nextColumn, nextRow);

        nextRow++;

        // calcula cuántos iconos caben verticalmente según el alto del escritorio
        float usableHeight = desktopArea.rect.height;
        int iconsPerColumn = Mathf.Max(1, Mathf.FloorToInt(usableHeight / (iconCellSize.y + iconPadding.y)));

        // si la columna está llena, pasamos a la siguiente
        if (nextRow >= iconsPerColumn)
        {
            nextRow = 0;
            nextColumn++;
        }

        return pos;
    }

    public void SpawnIcon(Vector2Int gridPos, string label, Sprite sprite)
    {
        if (!iconPrefab || desktopArea == null) return;

        GameObject go = Instantiate(iconPrefab, desktopArea);
        DesktopIcon di = go.GetComponent<DesktopIcon>();
        di.Setup(label, sprite, this);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = GridToPosition(gridPos);
        icons.Add(go);
    }

    public Vector2 GridToPosition(Vector2Int gridPos)
    {
        // coloca desde la esquina superior izquierda hacia abajo
        float startX = -desktopArea.rect.width / 2f + iconCellSize.x / 2f + iconPadding.x;
        float startY = desktopArea.rect.height / 2f - iconCellSize.y / 2f - iconPadding.y;

        float x = startX + gridPos.x * (iconCellSize.x + iconPadding.x);
        float y = startY - gridPos.y * (iconCellSize.y + iconPadding.y);

        return new Vector2(x, y);
    }

    public Vector2Int PositionToGrid(Vector2 anchoredPos)
    {
        float startX = -desktopArea.rect.width / 2f + iconCellSize.x / 2f + iconPadding.x;
        float startY = desktopArea.rect.height / 2f - iconCellSize.y / 2f - iconPadding.y;

        float dx = anchoredPos.x - startX;
        float dy = startY - anchoredPos.y;

        int gx = Mathf.Max(0, Mathf.FloorToInt(dx / (iconCellSize.x + iconPadding.x)));
        int gy = Mathf.Max(0, Mathf.FloorToInt(dy / (iconCellSize.y + iconPadding.y)));

        return new Vector2Int(gx, gy);
    }

    public void DeselectIcon()
    {
        if (selectedIcon != null) selectedIcon.SetSelected(false);
        selectedIcon = null;
    }

    public void SelectIcon(DesktopIcon icon)
    {
        if (selectedIcon == icon) return;
        DeselectIcon();
        selectedIcon = icon;
        if (selectedIcon != null) selectedIcon.SetSelected(true);
    }

    public bool IsGridOccupied(Vector2Int gridPos, DesktopIcon ignoreIcon = null)
    {
        foreach (var go in icons)
        {
            if (go == null) continue;
            DesktopIcon icon = go.GetComponent<DesktopIcon>();
            if (icon == null || icon == ignoreIcon) continue;

            Vector2Int otherGrid = PositionToGrid(icon.GetComponent<RectTransform>().anchoredPosition);
            if (otherGrid == gridPos)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class DesktopIconData
{
    public string label;
    public Sprite sprite;
    public Vector2Int gridPos = new Vector2Int(-1, -1);
    public GameObject windowApp;
    public bool isOpen;
}