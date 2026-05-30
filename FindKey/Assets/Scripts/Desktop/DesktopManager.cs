using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class: DesktopManager
/// Description: This script manages the desktop area of the FindKey game, allowing for the spawning and management of desktop icons. It handles the layout of icons in a grid format,
///              allowing for both predefined positions and automatic placement. The script also manages the selection and deselection of icons, as well as checking for occupied grid positions to
///              prevent overlapping icons. It references a prefab for the desktop icons and allows for customization of icon size and padding. The script ensures that icons are spawned after the
///              layout is calculated to properly position them on the desktop area.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class DesktopManager : MonoBehaviour
{
    public static DesktopManager Instance { get; private set; } // Singleton to global access

    [Header("Desktop Settings")]
    public RectTransform desktopArea;
    public GameObject iconPrefab;
    public Vector2 iconCellSize = new Vector2(80, 80); // Size of each icon cell (width and height)
    public Vector2 iconPadding = new Vector2(10, 20); // Space between icons (horizontal and vertical)

    [Header("Icon Data")]
    public DesktopIconData[] iconsToSpawn;

    [HideInInspector] public List<GameObject> icons = new List<GameObject>();
    [HideInInspector] public DesktopIcon selectedIcon;

    private int nextColumn = 0;
    private int nextRow = 0;

    private void Start()
    {
        StartCoroutine(SpawnIconsAfterLayout()); // Wait until the end of the frame to ensure layout is calculated before spawning icons
    }

    private IEnumerator SpawnIconsAfterLayout() // Coroutine to spawn icons after layout is calculated
    {
        yield return null;

        foreach (var data in iconsToSpawn) // Loop through the predefined icon data and spawn icons accordingly
        {
            if (data == null) continue;

            Vector2Int pos = data.gridPos;

            if (pos.x < 0 || pos.y < 0)
                pos = GetNextAutoGridPosition();

            SpawnIcon(pos, data.label, data.sprite); // Spawn the icon at the determined grid position with the specified label and sprite
        }
    }

    private Vector2Int GetNextAutoGridPosition() // Method to get the next available grid position for automatic icon placement
    {
        Vector2Int pos = new Vector2Int(nextColumn, nextRow);

        nextRow++;

        float usableHeight = desktopArea.rect.height;
        int iconsPerColumn = Mathf.Max(1, Mathf.FloorToInt(usableHeight / (iconCellSize.y + iconPadding.y)));

        if (nextRow >= iconsPerColumn) // Move to the next column if we've filled the current one
        {
            nextRow = 0;
            nextColumn++;
        }

        return pos;
    }

    public void SpawnIcon(Vector2Int gridPos, string label, Sprite sprite) // Method to spawn an icon at a specific grid position with a given label and sprite
    {
        if (!iconPrefab || desktopArea == null) return;

        GameObject go = Instantiate(iconPrefab, desktopArea); // Instantiate the icon prefab as a child of the desktop area
        DesktopIcon di = go.GetComponent<DesktopIcon>();
        di.Setup(label, sprite, this);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = GridToPosition(gridPos);
        icons.Add(go);
    }

    public Vector2 GridToPosition(Vector2Int gridPos) // Method to convert a grid position to an anchored position on the desktop area, taking into account icon size and padding
    {
        float startX = -desktopArea.rect.width / 2f + iconCellSize.x / 2f + iconPadding.x;
        float startY = desktopArea.rect.height / 2f - iconCellSize.y / 2f - iconPadding.y;

        float x = startX + gridPos.x * (iconCellSize.x + iconPadding.x);
        float y = startY - gridPos.y * (iconCellSize.y + iconPadding.y);

        return new Vector2(x, y);
    }

    public Vector2Int PositionToGrid(Vector2 anchoredPos) // Method to convert an anchored position back to a grid position, allowing for the calculation of which grid cell corresponds to a given position on the desktop area
    {
        float startX = -desktopArea.rect.width / 2f + iconCellSize.x / 2f + iconPadding.x;
        float startY = desktopArea.rect.height / 2f - iconCellSize.y / 2f - iconPadding.y;

        float dx = anchoredPos.x - startX;
        float dy = startY - anchoredPos.y;

        int gx = Mathf.Max(0, Mathf.FloorToInt(dx / (iconCellSize.x + iconPadding.x)));
        int gy = Mathf.Max(0, Mathf.FloorToInt(dy / (iconCellSize.y + iconPadding.y)));

        return new Vector2Int(gx, gy);
    }

    public void DeselectIcon() // Method to deselect the currently selected icon, if any, and reset its visual state
    {
        if (selectedIcon != null) selectedIcon.SetSelected(false);
        selectedIcon = null;
    }

    public void SelectIcon(DesktopIcon icon) // Method to select a specific icon, updating the selectedIcon reference and changing its visual state to indicate selection
    {
        if (selectedIcon == icon) return;
        DeselectIcon();
        selectedIcon = icon;
        if (selectedIcon != null) selectedIcon.SetSelected(true);
    }

    public bool IsGridOccupied(Vector2Int gridPos, DesktopIcon ignoreIcon = null) // Method to check if a specific grid position is already occupied by another icon, optionally ignoring a specific icon (useful for checking during drag operations)
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


/// <summary>
/// Class: DeskopIconData
/// Description: This class represents the data structure for a desktop icon in the FindKey game. It contains information about the icon's label, sprite, grid position on the desktop,
///              associated window application, and its current state (open, minimized). This data is used to initialize and manage desktop icons when they are spawned on the desktop area. The grid
///              position allows for both predefined placement and automatic positioning of icons based on available space. The windowApp reference can be used to link the icon to a specific
///              application window that it should open when interacted with.
/// </summary>
[System.Serializable]
public class DesktopIconData
{
    public string label;
    public Sprite sprite;
    public Vector2Int gridPos = new Vector2Int(-1, -1);
    public GameObject windowApp;
    public bool isOpen;
    public bool isMinimized;
    public GameObject windowInstance;
}