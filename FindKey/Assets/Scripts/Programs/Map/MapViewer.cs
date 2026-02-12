using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapViewer : MonoBehaviour
{
    [Header("Referencias")]
    public Image mapDisplay; // Arrastra aquí el componente Image de tu ventana Mapa
    public MoveAppManager moveAppManager; // Referencia al manager de movimientos

    [Header("Imágenes del Mapa")]
    // Arrastra aquí tus PNGs correspondientes a cada zona
    public Sprite mapStart;          // Inicio
    public Sprite mapPainting;       // Cuadro (Izquierda)
    public Sprite mapCorridor;       // Pasillo (Recto)
    public Sprite mapAxeArea;        // Hacha (Recto -> Derecha)
    public Sprite mapCatArea;        // Gato (Recto -> Recto)
    public Sprite mapUnknown;        // Por si se pierde (opcional)

    void Start()
    {
        if (moveAppManager == null)
            moveAppManager = FindObjectOfType<MoveAppManager>();

        DesktopManager dm = FindObjectOfType<DesktopManager>();
    }

    void Update()
    {
        if (moveAppManager == null) return;

        UpdateMapImage();
    }

    void UpdateMapImage()
    {
        // 1. Zona Inicio (Sin movimientos)
        if (moveAppManager.movementHistory.Count == 0)
        {
            SetImage(mapStart);
            return;
        }

        // 2. Zona Cuadro (Izquierda)
        if (MatchesSequence(Direction.Left))
        {
            SetImage(mapPainting);
            return;
        }

        // 3. Zona Hacha (Recto -> Derecha)
        if (MatchesSequence(Direction.Straight, Direction.Right))
        {
            SetImage(mapAxeArea);
            return;
        }

        // 4. Zona Gato (Recto -> Recto)
        if (MatchesSequence(Direction.Straight, Direction.Straight))
        {
            SetImage(mapCatArea);
            return;
        }

        // 5. Zona Pasillo (Recto)
        // Lo ponemos el último porque "Recto" es el inicio de "Recto->Derecha" y "Recto->Recto"
        if (MatchesSequence(Direction.Straight))
        {
            SetImage(mapCorridor);
            return;
        }

        // Si no coincide con nada conocido
        SetImage(mapUnknown);
    }

    // Función auxiliar para cambiar la imagen solo si es diferente (optimización)
    void SetImage(Sprite newSprite)
    {
        if (newSprite != null && mapDisplay.sprite != newSprite)
        {
            mapDisplay.sprite = newSprite;
        }
    }

    // Misma lógica que en tu SelectMove.cs para detectar la ruta
    private bool MatchesSequence(params Direction[] sequence)
    {
        List<Direction> history = moveAppManager.movementHistory;

        if (history.Count != sequence.Length) return false;

        for (int i = 0; i < sequence.Length; i++)
        {
            if (history[i] != sequence[i]) return false;
        }
        return true;
    }

    public void SetManager(MoveAppManager manager)
    {
        this.moveAppManager = manager;
        UpdateMapImage();
    }
}