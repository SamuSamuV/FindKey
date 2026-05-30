/// <summary>
/// Class: PersonalizeManager
/// Description: This script manages the personalization section of the FindKey game, specifically handling the display and functionality of wallpaper selection.
///              It references the WallpapersScript to change the desktop background when a wallpaper button is clicked. The script dynamically generates buttons
///              for each available wallpaper and assigns the appropriate functionality to change the background when a button is pressed. It also ensures that the
///              desktop background can be updated by linking it to the WallpapersScript at runtime.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using UnityEngine.UI;

public class PersonalizeManager : MonoBehaviour
{
    public Transform inventoryGrid;
    public GameObject wallpaperButtonPrefab;

    private WallpapersScript wallpapersScript;

    void Start()
    {
        wallpapersScript = FindObjectOfType<WallpapersScript>();

        GameObject backgroundObj = GameObject.Find("DesktopArea");

        // Link the desktop background to the WallpapersScript
        if (backgroundObj != null)
        {
            wallpapersScript.desktopBackground = backgroundObj.GetComponent<Image>();
        }

        // Spawn wallpaper buttons in the inventory grid
        SpawnWallpaperButtons();
    }

    public void SpawnWallpaperButtons()
    {
        // Clear existing buttons
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        if (wallpapersScript == null || wallpapersScript.availableWallpapers == null) return;

        // Create a button for each available wallpaper
        foreach (Sprite wallpaper in wallpapersScript.availableWallpapers)
        {
            GameObject newBtn = Instantiate(wallpaperButtonPrefab, inventoryGrid);

            Image btnImage = newBtn.transform.GetChild(0).GetComponent<Image>();

            // Set the button image to the wallpaper thumbnail
            if (btnImage != null)
            {
                btnImage.sprite = wallpaper;
                btnImage.preserveAspect = true;
            }

            // Add click listener to change the background when the button is clicked
            Button btn = newBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => wallpapersScript.ChangeBackground(wallpaper));
            }
        }
    }
}