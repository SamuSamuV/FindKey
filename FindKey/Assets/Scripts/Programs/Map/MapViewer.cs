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
        public StoryNode node;
        public Sprite image;
    }

    void Start()
    {
        AdventureManager adventure = FindObjectOfType<AdventureManager>();
        if (adventure != null) UpdateMap(adventure.currentNode);
        else if (defaultImage != null) mapDisplay.sprite = defaultImage;
    }

    public void UpdateMap(StoryNode currentNode)
    {
        if (currentNode == null) return;

        Debug.Log($"[MAPA] Intentando cambiar imagen para el nodo: {currentNode.name}");

        foreach (var entry in mapEntries)
        {
            if (entry.node == currentNode)
            {
                Debug.Log($"[MAPA] ¡Encontrado! Cambiando a imagen: {entry.image.name}");
                if (mapDisplay != null) mapDisplay.sprite = entry.image;
                return;
            }
        }

        Debug.LogWarning($"[MAPA] El nodo '{currentNode.name}' NO ESTÁ en la lista 'Map Entries'. Manteniendo imagen anterior.");
    }
}