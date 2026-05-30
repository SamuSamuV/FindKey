using UnityEngine;
using System.Collections.Generic;


public enum StoryAction // Enum to define the type of action associated with a StoryOption, which can be used to trigger specific behaviors in the Moves script when the player selects an option.
{
    None, TriggerCat, PickAxe, LookPainting, Die
}

public enum AudioChannel // Enum to define different audio channels for sound management, allowing for more granular control over audio playback in the game.
{
    Master, Front, Back, Left, Right
}

/// <summary>
/// Class: NodeSoundAction
/// Description: This class represents a sound action that can be triggered when the player enters a StoryNode. It contains information about which audio channel to play
///              the sound on and the settings for the sound itself. These sounds are designed to enhance the player's immersion and can be used for various effects, such as
///              background music, ambient noises, or specific sound effects related to the narrative. The sounds defined in this class will stop playing when the player moves
///              to a different StoryNode, allowing for dynamic audio changes throughout the game.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
[System.Serializable]
public class NodeSoundAction
{
    public AudioChannel channel = AudioChannel.Master;
    public SoundSettings soundSettings;
}

/// <summary>
/// Class: RandomNodeSoundAction
/// Description: This class represents a sound action that can be triggered randomly while the player is in a StoryNode. It includes settings for the audio channel, the sound to be played.
/// </summary>

[System.Serializable]
public class RandomNodeSoundAction
{
    public AudioChannel channel = AudioChannel.Master; // Audio channel to play the sound on
    public SoundSettings soundSettings;

    [Header("Intervalo de Tiempo (Segundos)")]
    public float minInterval = 5f;
    public float maxInterval = 15f;

    [Header("Probabilidad de sonar (%)")]
    [Range(0f, 1f)] public float playChance = 0.5f;
}

/// <summary>
/// Class: AmbientSoundAction
/// Description: This class represents a persistent ambient sound that can be triggered when the player enters a StoryNode. It includes a unique tag to identify the sound,
///              the audio channel to play it on, and the settings for the sound itself. These ambient sounds are designed to create a consistent atmosphere and can continue
///              playing across multiple StoryNodes until they are explicitly stopped or replaced by another sound with the same tag. This allows for dynamic and immersive audio
///              experiences that evolve as the player progresses through the game's narrative.
/// </summary>

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

/// <summary>
/// Class: StoryNode
/// Description: This class represents a node in the narrative structure of the FindKey game. Each StoryNode contains the text to be displayed, options for player choices, and various settings.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
[CreateAssetMenu(fileName = "NewStoryNode", menuName = "Adventure/Story Node")]
public class StoryNode : ScriptableObject
{
[TextArea(5, 10)]
    public string storyText;

    [Space(10)]
    [Header("Text Settings")]
    [Tooltip("Velocidad de escritura para este nodo. Si dejas 0, usará la velocidad normal del StoryLog.")]
    public float customTypingSpeed = 0f;

    [Tooltip("Si se marca, el jugador NO podrá saltar ni acelerar este texto pulsando Enter.")]
    public bool canNotBeSkipped = false;

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

    [Space(10)]
    [TextArea(3, 5)]
    [Tooltip("Si escribes algo aquí, la IA tomará la iniciativa y hablará sola al entrar a este nodo.")]
    public string aiProactivePrompt;

    [Tooltip("Fuerza a la IA a asimilar esta memoria y adaptar su nivel aunque sea inferior al actual (util para saltos en el arbol).")]
    public bool forceMemoryUpdate = false;
}

/// <summary>
/// Class: PopupData
/// Description: This class represents the data for a popup that can be displayed during a StoryNode. It includes settings for the delay before the popup appears, its duration,
///              position on the screen, the specific prefab to use for the popup, the title and message to be displayed, an optional image, and sound settings for any audio that
///              should accompany the popup.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
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

/// <summary>
/// Class: StoryOption
/// Description: This class represents an option that the player can choose when presented with a StoryNode.
///              Each StoryOption includes valid inputs that the player can type to select the option.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

[System.Serializable]
public class StoryOption
{
    public CommandVocabulary validInputs;
    public StoryNode nextNode;
    public StoryAction actionType;

    [Space(10)]
    [Header("Bloquear nodo")]
    [Tooltip("Si marcas esto, el jugador no podra volver a pasar por este nodo.")]
    public bool disableIfVisited = false;

    [Tooltip("Opcional: Si el jugador intenta usar esta opción pero está bloqueada, lo enviará a este nodo alternativo en su lugar (Ej: 'Ya has explorado esa zona').")]
    public StoryNode alternateNodeIfBlocked;
}