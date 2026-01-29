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

        if (backgroundObj != null)
        {
            wallpapersScript.desktopBackground = backgroundObj.GetComponent<Image>();
        }

        SpawnWallpaperButtons();
    }

    public void SpawnWallpaperButtons()
    {
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        if (wallpapersScript == null || wallpapersScript.availableWallpapers == null) return;

        foreach (Sprite wallpaper in wallpapersScript.availableWallpapers)
        {
            GameObject newBtn = Instantiate(wallpaperButtonPrefab, inventoryGrid);

            Image btnImage = newBtn.transform.GetChild(0).GetComponent<Image>();

            if (btnImage != null)
            {
                btnImage.sprite = wallpaper;
                btnImage.preserveAspect = true;
            }

            Button btn = newBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => wallpapersScript.ChangeBackground(wallpaper));
            }
        }
    }
}