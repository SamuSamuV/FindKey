using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesktopManager : MonoBehaviour
{
    [Header("Desktop Settings")]
    public RectTransform desktopArea;
    public GameObject iconPrefab;
    public Vector2Int gridSize = new Vector2Int(19, 11);
    public Vector2 iconCellSize = new Vector2(64, 64);
    public Vector2 iconPadding = new Vector2(8, 24);

    [Header("Icon Data (Configurable desde el Inspector)")]
    public DesktopIconData[] iconsToSpawn;

    [HideInInspector] public List<GameObject> icons = new List<GameObject>();
    [HideInInspector] public DesktopIcon selectedIcon;

    private Vector2Int nextAutoGridPos = Vector2Int.zero;

    private void Start()
    {
        // 🔹 Esperar un frame para asegurar que el layout del Canvas ya está calculado
        StartCoroutine(SpawnIconsAfterLayout());
    }

    private IEnumerator SpawnIconsAfterLayout()
    {
        yield return null; // espera un frame
        foreach (var data in iconsToSpawn)
        {
            if (data == null) continue;

            Vector2Int pos = data.gridPos;

            // Si no tiene posición definida, usar autocolocación
            if (pos.x < 0 || pos.y < 0)
                pos = GetNextAutoGridPosition();

            SpawnIcon(pos, data.label, data.sprite);
        }
    }

    private Vector2Int GetNextAutoGridPosition()
    {
        Vector2Int pos = nextAutoGridPos;

        // baja verticalmente
        nextAutoGridPos.y++;

        // cuando llega abajo, pasa a la siguiente columna
        if (nextAutoGridPos.y >= gridSize.y)
        {
            nextAutoGridPos.y = 0;
            nextAutoGridPos.x++;
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
        float width = desktopArea.rect.width;
        float height = desktopArea.rect.height;
        float cellW = width / gridSize.x;
        float cellH = height / gridSize.y;

        float x = (cellW * gridPos.x) + cellW / 2 - width / 2;
        float y = height / 2 - (cellH * gridPos.y) - cellH / 2;

        return new Vector2(x, y);
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
}

[System.Serializable]
public class DesktopIconData
{
    public string label;
    public Sprite sprite;
    public Vector2Int gridPos = new Vector2Int(-1, -1);
}
