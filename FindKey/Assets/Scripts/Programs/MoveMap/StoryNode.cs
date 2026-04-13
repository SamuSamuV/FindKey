using UnityEngine;
using System.Collections.Generic;

public enum StoryAction
{
    None, TriggerCat, PickAxe, LookPainting, Die
}

public enum AudioChannel
{
    Master, Front, Back, Left, Right
}

[System.Serializable]
public class NodeSoundAction
{
    public AudioChannel channel = AudioChannel.Master;
    public SoundSettings soundSettings;
}

[System.Serializable]
public class RandomNodeSoundAction
{
    public AudioChannel channel = AudioChannel.Master;
    public SoundSettings soundSettings;

    [Header("Intervalo de Tiempo (Segundos)")]
    public float minInterval = 5f;
    public float maxInterval = 15f;

    [Header("Probabilidad de sonar (%)")]
    [Range(0f, 1f)] public float playChance = 0.5f;
}

[System.Serializable]
public class AmbientSoundAction
{
    [Tooltip("Etiqueta única. Ej: 'Lluvia' o 'MusicaFondo'.")]
    public string ambientTag;

    public AudioChannel channel = AudioChannel.Master;
    public SoundSettings soundSettings;

    [Tooltip("Marca esto para DETENER el sonido que tenga este Tag sin poner uno nuevo.")]
    public bool stopAmbient = false;
}

[CreateAssetMenu(fileName = "NewStoryNode", menuName = "Adventure/Story Node")]
public class StoryNode : ScriptableObject
{
[TextArea(5, 10)]
    public string storyText;

    [Space(10)]
    [Header("Text Settings")]
    [Tooltip("Velocidad de escritura para este nodo. Si dejas 0, usará la velocidad normal del StoryLog.")]
    public float customTypingSpeed = 0f;

    public List<StoryOption> options;
    [Space(20)]
    [Header("Popups Sequence")]
    public List<PopupData> popups;

    [Space(20)]
    [Header("Node Sounds (Mueren al cambiar de nodo)")]
    public List<NodeSoundAction> nodeSounds;

    [Space(20)]
    [Header("Random Ambient Sounds")]
    public List<RandomNodeSoundAction> randomSounds;

    [Space(20)]
    [Header("Persistent Ambient Sounds (Sobreviven entre nodos)")]
    public List<AmbientSoundAction> ambientSounds;

    [Space(20)]
    [Header("System OS Event")]
    [Tooltip("Arrastra aquí un GameEvent. Se disparará automáticamente en cuanto el jugador llegue a este nodo.")]
    public GameEvent onEnterEvent;

    [Space(20)]
    [Header("Actualización de Memoria IA (Fases)")]
    [Tooltip("Nivel de esta memoria. La IA solo actualizará si este número es MAYOR al que ya tiene.")]
    public int aiMemoryLevel = 0;

    [TextArea(3, 5)]
    [Tooltip("La IA aprenderá esto permanentemente al llegar a este nodo. Ej: 'Fase 2: El gato ha muerto'.")]
    public string aiMemoryUpdate;
}

[System.Serializable]
public class PopupData
{
    public float delayBeforeSpawn = 0f;
    public float duration = 0f;
    public Vector2 position;
    public GameObject specificPrefab;
    public string title;
    [TextArea(3, 5)]
    public string message;
    public Sprite image;
    public SoundSettings sound;
}

[System.Serializable]
public class StoryOption
{
    public CommandVocabulary validInputs;
    public StoryNode nextNode;
    public StoryAction actionType;
}