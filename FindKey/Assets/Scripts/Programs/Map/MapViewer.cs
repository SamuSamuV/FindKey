using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapViewer : MonoBehaviour
{
    [Header("Referencias")]
    public Image mapDisplay;

    [Header("Configuración Mapa")]
    public List<MapEntry> mapEntries;

    public Sprite defaultImage;

    [System.Serializable]
    public struct MapEntry
    {
        public string name;
        public List<StoryNode> nodes;
        public Sprite image;
    }

    void Start()
    {
        AdventureManager adventure = FindObjectOfType<AdventureManager>();
        if (adventure != null) UpdateMap(adventure.currentNode);
        else if (defaultImage != null && mapDisplay.sprite == null) mapDisplay.sprite = defaultImage;
    }

    public void UpdateMap(StoryNode currentNode)
    {
        if (currentNode == null) return;

        foreach (var entry in mapEntries)
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