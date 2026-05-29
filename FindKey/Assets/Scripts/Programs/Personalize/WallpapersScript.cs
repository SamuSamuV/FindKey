/// <summary>
/// Class: WallpapersScript
/// Description: This script manages the desktop background in the personalization section of the FindKey game. It allows changing the wallpaper by updating the sprite of
///              the Image component that represents the desktop background. The script holds a reference to the Image component and an array of available wallpapers,
///              which can be set in the Unity Editor. When the ChangeBackground function is called with a new wallpaper sprite, it updates the desktop background accordingly.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

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