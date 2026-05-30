using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Class: MapViewer
/// Description: This script manages the display of the map in the FindKey game. It allows for updating the map image based on the current story node.
///              The script references an Image component for displaying the map and a list of MapEntry structs that associate specific story nodes with corresponding map images.
///              When the current story node changes, the script checks if it matches any of the nodes in the MapEntry list and updates the displayed map image accordingly.
///              If no matching entry is found, it can optionally display a default image.
///              This functionality helps players visualize their progress and location within the game's narrative.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class MapViewer : MonoBehaviour
{
    [Header("Referencias")]
    public Image mapDisplay;

    [Header("Configuración Mapa")]
    public List<MapEntry> mapEntries;

    public Sprite defaultImage;

    [System.Serializable]
    public struct MapEntry // Struct to associate story nodes with map images
    {
        public string name;
        public List<StoryNode> nodes;
        public Sprite image;
    }

    void Start() // Initialize the map display based on the current story node when the game starts
    {
        AdventureManager adventure = FindObjectOfType<AdventureManager>();
        if (adventure != null) UpdateMap(adventure.currentNode);
        else if (defaultImage != null && mapDisplay.sprite == null) mapDisplay.sprite = defaultImage;
    }

    public void UpdateMap(StoryNode currentNode) // Method to update the map display based on the current story node
    {
        if (currentNode == null) return;

        foreach (var entry in mapEntries) // Loop through each MapEntry to find a matching story node
        {
            if (entry.nodes.Contains(currentNode))
            {
                if (mapDisplay != null && mapDisplay.sprite != entry.image)
                {
                    mapDisplay.sprite = entry.image;
                }
                return;
            }
        }

    }
}