using UnityEngine;
using UnityEngine.UI;

public class WallpapersScript : MonoBehaviour
{
    public Image desktopBackground;
    public Sprite[] availableWallpapers;

    public void ChangeBackground(Sprite newWallpaper)
    {
        desktopBackground.sprite = newWallpaper;
    }
}