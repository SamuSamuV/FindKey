using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStoryNode", menuName = "Adventure/Story Node")]
public class StoryNode : ScriptableObject
{
    [TextArea(5, 10)]
    public string storyText; // El texto que se muestra en pantalla

    // Lista de opciones posibles desde este lugar
    public List<StoryOption> options;
}

[System.Serializable]
public class StoryOption
{
    public string commandKeyword; // Lo que el jugador escribe (ej: "straight", "right", "pick axe")
    public StoryNode nextNode;    // A dónde te lleva (arrastrar otro nodo aquí)

    [Tooltip("Opcional: Si quieres que pase algo especial (ej: 'EncuentroGato')")]
    public string specialActionID;
}