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

        SetInAppWallpapers();
    }

    public void SetInAppWallpapers()
    {
        if (wallpapersScript == null || wallpapersScript.availableWallpapers == null) return;

        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (Sprite wallpaper in wallpapersScript.availableWallpapers)
        {
            GameObject newBtn = Instantiate(wallpaperButtonPrefab, inventoryGrid);

            Image btnImage = newBtn.GetComponent<Image>();
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