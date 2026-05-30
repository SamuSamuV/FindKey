using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class: AdventureManager
/// Description: This script serves as the central manager for the adventure mode of the FindKey game. It handles the narrative flow, player input, audio management, and
///              visual updates based on the current story node. The AdventureManager processes player choices, updates the story log, manages audio sources for music and
///              sound effects, and controls the display of popups and other visual elements. It also interacts with other components such as Moves and MapViewer to ensure
///              a cohesive gameplay experience. The script is designed to be flexible and extensible, allowing for complex storytelling and dynamic responses to player actions
///              throughout the adventure mode.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class AdventureManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public StoryLog storyLog;
    public Moves movesScript;
    public MapViewer mapViewer;

    public Transform popupContainer;
    public GameObject defaultPopupPrefab;

    [Header("Audio GameObjects (Auto-detected)")]
    public GameObject audioMasterObj;
    public GameObject audioFrontObj;
    public GameObject audioBackObj;
    public GameObject audioLeftObj;
    public GameObject audioRightObj;

    public StoryNode currentNode;
    public StoryNode nodeAfterCatWin;

    private Coroutine popupSequenceCoroutine;

    private List<AudioSource> activeAudioSources = new List<AudioSource>(); // For node-specific sounds that should persist across nodes until changed
    private List<Coroutine> activeRandomSoundCoroutines = new List<Coroutine>(); // To keep track of active random sound coroutines so they can be stopped when needed
    private List<GameObject> activeRandomSoundObjects = new List<GameObject>(); // To keep track of the GameObjects created for random sounds so they can be destroyed when needed

    private Dictionary<string, AudioSource> activeAmbientSources = new Dictionary<string, AudioSource>(); // Key: ambientTag, Value: AudioSource - to manage ambient sounds that can persist across nodes and be individually controlled
    private Dictionary<string, AmbientSoundAction> persistentAmbientsData = new Dictionary<string, AmbientSoundAction>(); // To store the data for persistent ambient sounds so they can be restarted if the player returns to a node with the same ambient or if the scene is reloaded

    private bool isInitialized = false;

    private List<StoryNode> visitedNodes = new List<StoryNode>(); // To track visited nodes for typing speed and option blocking

    [HideInInspector] public string globalAIMemory = "";
    [HideInInspector] public int globalAIMemoryLevel = -1;

    void Start() // Initialization moved to a separate method to allow for better control when enabling/disabling the script
    {
        audioMasterObj = GameObject.Find("Main Camera");
        audioFrontObj = GameObject.Find("FrontSoundBox");
        audioBackObj = GameObject.Find("BackSoundBox");
        audioLeftObj = GameObject.Find("LeftSoundBox");
        audioRightObj = GameObject.Find("RightSoundBox");

        if (audioMasterObj == null) audioMasterObj = gameObject;
        if (audioFrontObj == null) audioFrontObj = gameObject;
        if (audioBackObj == null) audioBackObj = gameObject;
        if (audioLeftObj == null) audioLeftObj = gameObject;
        if (audioRightObj == null) audioRightObj = gameObject;

        if (popupContainer == null)
        {
            GameObject containerObj = GameObject.FindGameObjectWithTag("DeskopCanvas");
            if (containerObj != null) popupContainer = containerObj.transform;
        }

        isInitialized = true;

        UpdateStoryVisuals(); // Ensure visuals are updated at the start
        inputField.onSubmit.AddListener(ProcessInput); // Subscribe to input field submit event
        TryUpdateMap(); // Ensure map is updated at the start
    }

    void OnEnable() // When re-enabling the script, we need to restore any persistent ambient sounds and update the audio for the current node in case we've returned to a previously visited node
    {
        if (isInitialized)
        {
            List<AmbientSoundAction> ambientsToRestore = new List<AmbientSoundAction>(persistentAmbientsData.Values);
            persistentAmbientsData.Clear();

            foreach (var ambient in ambientsToRestore) // This will restart the ambient sounds that were active before, ensuring they continue playing if we return to a node that had them or if the scene was reloaded
            {
                StartAmbientSound(ambient);
            }

            if (currentNode != null)
            {
                UpdateNodeAudio(currentNode.nodeSounds); // This will ensure that if we return to a node with specific node sounds, they will be correctly updated (e.g., if they were changed while we were away)
                UpdateRandomAudio(currentNode.randomSounds); // This will restart the random sound coroutines for the current node if we return to it, ensuring the audio experience remains consistent with the node's design
            }
        }
    }

    void OnDisable() // When disabling the script, we want to stop all audio and coroutines to ensure a clean state when we come back
    {
        foreach (Coroutine c in activeRandomSoundCoroutines) // Stop all active random sound coroutines to prevent them from running while the script is disabled
        {
            if (c != null) StopCoroutine(c);
        }
        activeRandomSoundCoroutines.Clear();

        foreach (AudioSource oldSource in activeAudioSources) // Stop and destroy all active node-specific audio sources to ensure they don't continue playing while the script is disabled
        {
            if (oldSource != null)
            {
                AudioFader fader = oldSource.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject(); // If there's an AudioFader, use it to fade out the audio smoothly before destroying the GameObject
                else
                {
                    oldSource.Stop();
                    Destroy(oldSource.gameObject);
                }
            }
        }
        activeAudioSources.Clear(); // Clear the list of active audio sources to ensure we have a clean slate when we come back

        foreach (GameObject randomObj in activeRandomSoundObjects) // Destroy all GameObjects created for random sounds to ensure they don't continue playing while the script is disabled
        {
            if (randomObj != null)
            {
                AudioFader fader = randomObj.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject();
                else Destroy(randomObj);
            }
        }
        activeRandomSoundObjects.Clear(); // Clear the list of active random sound objects to ensure we have a clean slate when we come back

        foreach (var kvp in activeAmbientSources) // Stop all active ambient audio sources to ensure they don't continue playing while the script is disabled.
                                                  // We keep track of the ambient data in persistentAmbientsData so we can restart them if we come back to a node that has them
                                                  // or if the scene is reloaded.
        {
            if (kvp.Value != null)
            {
                AudioFader fader = kvp.Value.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject();
                else Destroy(kvp.Value.gameObject);
            }
        }
        activeAmbientSources.Clear(); // We clear the active ambient sources but not the persistentAmbientsData, allowing us to restore them when we come back to a node that has them or if the scene is reloaded
    }

    /// <summary>
    /// Updates all visual, auditory, and narrative elements of the interface based on the current node data (StoryNode).
    /// Triggers on-enter events, pop-ups, and updates the AI memory.
    /// </summary>
    void UpdateStoryVisuals() // This method is responsible for updating all the visuals and audio based on the current story node. It sets the story text, manages popups
                              // updates audio sources, and triggers any events or AI updates associated with the node. This method is called whenever we load a new node to ensure
                              // that all aspects of the game reflect the current state of the story.
    {
        if (currentNode != null)
        {
            storyLog.canSkipCurrentText = !currentNode.canNotBeSkipped;

            float speedToUse = currentNode.customTypingSpeed;

            if (visitedNodes.Contains(currentNode)) // If we've been to this node before, we set the typing speed to a very low value to effectively skip the animation while still allowing for the possibility to show it if needed (e.g., if the player wants to re-read the text with the animation)
            {
                speedToUse = 0.001f;
            }

            else
            {
                visitedNodes.Add(currentNode);
            }

            storyLog.SetTextAnimated(currentNode.storyText, speedToUse);

            if (popupSequenceCoroutine != null) StopCoroutine(popupSequenceCoroutine);

            if (currentNode.popups != null && currentNode.popups.Count > 0) // If the new node has popups to show, start the coroutine to display them in sequence. If we were already showing popups from a previous node, they will be stopped immediately when we load the new node, ensuring that only the popups relevant to the current node are displayed.
            {
                popupSequenceCoroutine = StartCoroutine(RunPopupSequence(currentNode.popups));
            }

            //All this Updates are separated into different methods to keep things organized and allow for better control over each aspect (e.g., we can choose to only update audio without touching visuals if needed in the future)
            UpdateNodeAudio(currentNode.nodeSounds);
            UpdateRandomAudio(currentNode.randomSounds);

            UpdateAmbientAudio(currentNode.ambientSounds);

            UpdateAIMemory(currentNode.aiMemoryUpdate, currentNode.aiMemoryLevel, currentNode.forceMemoryUpdate);

            UpdateAISituationContext(currentNode.storyText);

            if (currentNode.onEnterEvent != null && EventManager.Instance != null) // Trigger any event associated with entering this node. This allows for a wide range of possibilities, such as changing the game state, affecting NPC behavior, or anything else that can be achieved through the event system.
            {
                EventManager.Instance.TriggerEvent(currentNode.onEnterEvent);
            }

            if (!string.IsNullOrEmpty(currentNode.aiProactivePrompt)) // If the node has a proactive AI prompt, we force all AIs to process it immediately. This allows for dynamic AI responses that are directly tied to the narrative and can react to specific story developments in a more urgent manner than the regular memory injection.
            {
                BaseAIScript[] allAIs = FindObjectsOfType<BaseAIScript>(true);
                foreach (BaseAIScript ai in allAIs)
                {
                    ai.ForceProactiveMessage(currentNode.aiProactivePrompt);
                }
            }
        }
    }

    /// <summary>
    /// Manages the ambient audio tracks for the current node.
    /// Performs smooth transitions (fade-in/fade-out) and maintains sound persistence if the player returns to previous nodes.
    /// </summary>
    /// <param name="newAmbients">List of ambient sound configurations to play.</param>
    void UpdateAmbientAudio(List<AmbientSoundAction> newAmbients) // This method manages the ambient sounds for the current node. It checks the new list of ambient sounds
                                                                  // against the currently active ones, stopping any that are no longer needed and starting any new ones.
                                                                  // It also updates the settings of existing ambient sounds if they are still relevant but have changes in their settings.
                                                                  // This allows for a dynamic and responsive ambient audio experience that can change as the player moves through different nodes in the story.
    {
        if (newAmbients == null) return;

        foreach (var ambient in newAmbients) // We loop through the new list of ambient sounds for the current node to determine what needs to be started, stopped, or updated. This allows us to ensure that the ambient audio always reflects the current narrative context and can adapt to changes in the story as needed.
        {
            if (string.IsNullOrEmpty(ambient.ambientTag)) continue;

            if (ambient.stopAmbient)
            {
                StopAmbientSound(ambient.ambientTag);
                continue;
            }

            if (ambient.soundSettings == null || !ambient.soundSettings.IsValid()) continue;

            if (activeAmbientSources.TryGetValue(ambient.ambientTag, out AudioSource existingSource)) // We check if there's already an active ambient sound with the same tag. If there is, we check if it's the same clip. If it is the same clip, we just update the settings (volume, pitch, etc.) without restarting it, allowing for seamless transitions when only the settings change. If it's a different clip, we stop the existing one and start the new one.
            {
                if (existingSource != null && existingSource.clip == ambient.soundSettings.clip)
                {
                    existingSource.volume = ambient.soundSettings.volume;
                    existingSource.pitch = ambient.soundSettings.pitch;
                    persistentAmbientsData[ambient.ambientTag] = ambient;
                    continue;
                }

                else
                {
                    StopAmbientSound(ambient.ambientTag);
                }
            }

            StartAmbientSound(ambient); // If we reach this point, it means we need to start the new ambient sound because it's either new or has a different clip than the existing one. This ensures that the correct ambient audio is always playing for the current node, enhancing the atmosphere and immersion of the game.
        }
    }

    void StartAmbientSound(AmbientSoundAction ambient) // This method is responsible for starting a new ambient sound based on the provided AmbientSoundAction data. It creates a new GameObject for the audio source, sets it up according to the specified channel and sound settings, and keeps track of it in the activeAmbientSources dictionary so it can be managed (e.g., stopped) later as needed.
    {
        GameObject targetObj = GetChannelObject(ambient.channel);
        GameObject soundObj = new GameObject("AmbientAudio_" + ambient.ambientTag);
        soundObj.transform.SetParent(targetObj.transform, false);

        AudioSource newSource = soundObj.AddComponent<AudioSource>();
        if (ambient.channel != AudioChannel.Master) newSource.spatialBlend = 1f;

        ambient.soundSettings.PlayOn(newSource, false);

        activeAmbientSources[ambient.ambientTag] = newSource;
        persistentAmbientsData[ambient.ambientTag] = ambient;
    }

    void StopAmbientSound(string tag) // This method is responsible for stopping an active ambient sound based on its tag. It checks if there's an active AudioSource associated with the tag, and if so, it stops it (using an AudioFader if available for a smooth fade-out) and removes it from the activeAmbientSources dictionary. It also removes the corresponding data from persistentAmbientsData to ensure that it won't be restarted if we return to a node that had this ambient sound or if the scene is reloaded.
    {
        if (activeAmbientSources.TryGetValue(tag, out AudioSource source))
        {
            if (source != null)
            {
                AudioFader fader = source.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject();
                else Destroy(source.gameObject);
            }
            activeAmbientSources.Remove(tag);
        }
        persistentAmbientsData.Remove(tag);
    }

    void UpdateRandomAudio(List<RandomNodeSoundAction> randomSounds) // This method manages the random sounds for the current node. It stops any existing random sound coroutines and destroys their associated GameObjects to ensure a clean slate. Then, it starts new coroutines for each random sound defined in the current node, allowing for dynamic and unpredictable audio elements that can enhance the atmosphere and immersion of the game.
    {
        foreach (Coroutine c in activeRandomSoundCoroutines) // We loop through all active random sound coroutines and stop them to ensure that we don't have any random sounds from a previous node still playing when we load a new node. This allows for a clean transition between nodes and ensures that the random audio experience is always relevant to the current narrative context.
        {
            if (c != null) StopCoroutine(c);
        }
        activeRandomSoundCoroutines.Clear();

        activeRandomSoundObjects.RemoveAll(item => item == null);

        if (randomSounds != null && randomSounds.Count > 0) // If the new node has random sounds defined, we loop through them and start a coroutine for each one to handle their random playback. This allows for a more dynamic audio experience that can change each time the player visits the node, as the random sounds will play at different intervals based on the settings defined in the RandomNodeSoundAction data.
        {
            foreach (var randomSound in randomSounds) // We loop through the list of random sounds defined for the current node and start a coroutine for each one to handle its random playback. This allows for a more dynamic and immersive audio experience, as these sounds can play at unpredictable intervals based on the settings defined in the RandomNodeSoundAction data.
            {
                if (randomSound.soundSettings != null && randomSound.soundSettings.IsValid())
                {
                    Coroutine newCoroutine = StartCoroutine(RandomSoundRoutine(randomSound));
                    activeRandomSoundCoroutines.Add(newCoroutine);
                }
            }
        }
    }

    IEnumerator RandomSoundRoutine(RandomNodeSoundAction randomData) // This coroutine handles the random playback of a single sound based on the settings defined in the RandomNodeSoundAction data. It waits for a random interval between the specified minimum and maximum, then rolls a random chance to determine whether to play the sound. If it decides to play the sound, it creates a new GameObject with an AudioSource, plays the sound, and ensures that the GameObject is destroyed after the sound has finished playing (taking into account any fade-in or fade-out durations).
    {
        float safeMin = Mathf.Max(0f, randomData.minInterval);
        float safeMax = Mathf.Max(safeMin, randomData.maxInterval);

        float waitTime = Random.Range(safeMin, safeMax);
        if (waitTime > 0) yield return new WaitForSeconds(waitTime); // Wait for a random interval before attempting to play the sound, allowing for unpredictable audio elements that can enhance the atmosphere and immersion of the game.

        float roll = Random.value;
        string clipName = randomData.soundSettings.clip.name;

        if (roll <= randomData.playChance) // Roll to determine if we should play the sound based on the defined play chance. This adds an additional layer of randomness, as even after waiting for a random interval, the sound may not play every time, creating a more dynamic and varied audio experience for the player.
        {
            GameObject targetObj = GetChannelObject(randomData.channel);

            GameObject randomSoundObj = new GameObject("RandomAudio_" + clipName);
            randomSoundObj.transform.SetParent(targetObj.transform, false);

            AudioSource newSource = randomSoundObj.AddComponent<AudioSource>();
            if (randomData.channel != AudioChannel.Master)
            {
                newSource.spatialBlend = 1f;
            }

            randomData.soundSettings.PlayOn(newSource, false);
            newSource.loop = false;

            activeRandomSoundObjects.Add(randomSoundObj);

            float duration = randomData.soundSettings.clip.length / Mathf.Max(randomData.soundSettings.pitch, 0.01f);
            float totalTime = duration + randomData.soundSettings.fadeInDuration + randomData.soundSettings.fadeOutDuration + 0.5f;
            Destroy(randomSoundObj, totalTime);
        }
    }

    void UpdateNodeAudio(List<NodeSoundAction> newSounds) // This method manages the node-specific sounds for the current story node. It checks the new list of node sounds against the currently active audio sources, updating settings for existing sources that are still relevant, stopping any sources that are no longer needed, and starting new sources for any new sounds defined in the current node. This allows for a dynamic and responsive audio experience that can change as the player moves through different nodes in the story, ensuring that the audio always reflects the current narrative context.
    {
        if (newSounds == null) newSounds = new List<NodeSoundAction>();

        List<AudioSource> sourcesToKeep = new List<AudioSource>();

        foreach (var action in newSounds) // We loop through the new list of node sounds for the current node to determine what needs to be updated, started, or stopped. This allows us to ensure that the node-specific audio always reflects the current narrative context and can adapt to changes in the story as needed.
        {
            if (action.soundSettings == null || !action.soundSettings.IsValid()) continue;

            AudioSource existingSource = activeAudioSources.Find(s => s != null && s.clip == action.soundSettings.clip);

            if (existingSource != null) // If there's already an active audio source playing the same clip, we just update its settings (volume, pitch, loop, etc.) without restarting it. This allows for seamless transitions when only the settings change, ensuring that the audio experience remains smooth and consistent with the current node's design.
            {
                existingSource.volume = action.soundSettings.volume;
                existingSource.pitch = action.soundSettings.pitch;
                existingSource.loop = action.soundSettings.loop;

                AudioFader fader = existingSource.GetComponent<AudioFader>();
                if (fader != null) fader.storedFadeOutTime = action.soundSettings.fadeOutDuration;

                if (!existingSource.isPlaying) existingSource.Play();

                sourcesToKeep.Add(existingSource);
                activeAudioSources.Remove(existingSource);
            }

            else // If we reach this point, it means we need to start a new audio source for this sound because there's no existing source playing the same clip. This ensures that all the node-specific sounds defined for the current node are correctly played, enhancing the atmosphere and immersion of the game.
            {
                GameObject targetObj = GetChannelObject(action.channel);
                GameObject soundObj = new GameObject("NodeAudio_" + action.soundSettings.clip.name);
                soundObj.transform.SetParent(targetObj.transform, false);

                AudioSource newSource = soundObj.AddComponent<AudioSource>();

                if (action.channel != AudioChannel.Master)
                {
                    newSource.spatialBlend = 1f;
                }

                action.soundSettings.PlayOn(newSource, false);
                sourcesToKeep.Add(newSource);
            }
        }

        foreach (AudioSource oldSource in activeAudioSources) // After processing all the new node sounds, any audio sources that are still in the activeAudioSources list are ones that are no longer needed for the current node and should be stopped and removed. This ensures that we don't have any lingering audio sources from previous nodes that are no longer relevant, keeping the audio experience clean and focused on the current narrative context.
        {
            if (oldSource != null)
            {
                AudioFader fader = oldSource.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject();
                else
                {
                    oldSource.Stop();
                    Destroy(oldSource.gameObject);
                }
            }
        }

        activeAudioSources = sourcesToKeep;
    }

    void UpdateAIMemory(string newMemory, int memoryLevel, bool forceUpdate) // This method manages the global AI memory for the current story node. It checks the new memory string and its associated memory level against the existing global AI memory and its level. If the new memory is not empty and has a higher memory level than the current global AI memory (or if forceUpdate is true), it updates the global AI memory and injects it into all active AI scripts in the scene. This allows for a dynamic and responsive AI behavior that can change based on the narrative context and player actions, with the ability to control how significant a memory update is through the memory level system.
    {
        if (string.IsNullOrEmpty(newMemory)) return;

        if (memoryLevel > globalAIMemoryLevel || forceUpdate)
        {
            globalAIMemoryLevel = memoryLevel;
            globalAIMemory = newMemory;

            BaseAIScript[] allAIs = FindObjectsOfType<BaseAIScript>(true);
            foreach (BaseAIScript ai in allAIs)
            {
                ai.InjectMemory(newMemory, memoryLevel, forceUpdate);
            }
        }
    }

    GameObject GetChannelObject(AudioChannel channel) // This method returns the appropriate GameObject for a given audio channel. This allows us to organize our audio sources in the scene hierarchy based on their channel, which can be useful for spatial audio and for keeping things organized. If a specific channel GameObject is not assigned, it defaults to using the main GameObject of the AdventureManager.
    {
        switch (channel)
        {
            case AudioChannel.Front: return audioFrontObj;
            case AudioChannel.Back: return audioBackObj;
            case AudioChannel.Left: return audioLeftObj;
            case AudioChannel.Right: return audioRightObj;
            case AudioChannel.Master:
            default: return audioMasterObj;
        }
    }

    IEnumerator RunPopupSequence(List<PopupData> popups) // This coroutine handles the sequential display of popups for the current story node. It loops through the list of PopupData, waiting for the specified delay before spawning each popup, and then calls the method to spawn the popup with the appropriate data. This allows for a dynamic and engaging way to present additional information, images, or sounds to the player in a way that complements the narrative and enhances the overall experience.
    {
        foreach (PopupData data in popups)
        {
            if (data.delayBeforeSpawn > 0) yield return new WaitForSeconds(data.delayBeforeSpawn);
            SpawnSinglePopup(data);
        }
    }

    void SpawnSinglePopup(PopupData data) // This method is responsible for spawning a single popup based on the provided PopupData. It determines which prefab to use (either a specific one defined in the PopupData or a default one), instantiates it as a child of the popup container, and then sets it up with the provided data. This allows for flexibility in the types of popups that can be displayed, as well as a consistent way to initialize them with the necessary information.
    {
        GameObject prefabToUse = data.specificPrefab != null ? data.specificPrefab : defaultPopupPrefab;

        if (prefabToUse != null)
        {
            GameObject newPopup = Instantiate(prefabToUse, popupContainer);
            PopupController controller = newPopup.GetComponent<PopupController>();
            if (controller != null) controller.Setup(data);
        }
    }

    /// <summary>
    /// Processes the text entered by the player in the input field.
    /// Compares the input with the flexible vocabulary (synonyms) of the current node to determine if there is a match and execute the next narrative action.
    /// </summary>
    /// <param name="input">Text command entered by the player.</param>
    void ProcessInput(string input) // This method processes the player's input from the input field. It first checks if the story log is currently typing out text and if the input is empty; if so, it tries to skip the typing animation. If the input is not empty, it checks the current story node's options to see if any of them match the player's input (taking into account synonyms). If a matching option is found, it executes that option, which may involve changing the current node and updating visuals and audio accordingly. If no matching option is found, it shows an error message to the player. This method is central to how player choices are processed and how they affect the flow of the story.
    {
        if (storyLog.IsTyping && string.IsNullOrEmpty(input.Trim()))
        {
            storyLog.TrySkipTyping();
            inputField.text = "";
            inputField.ActivateInputField();
            return;
        }

        if (string.IsNullOrEmpty(input.Trim()) || currentNode == null) return;

        input = input.ToLower().Trim();
        inputField.text = "";
        inputField.ActivateInputField();

        foreach (var option in currentNode.options) // We loop through the options of the current node to find one that matches the player's input. This allows for a flexible command system where each option can have multiple valid inputs (synonyms) that can trigger it, making it more user-friendly and accommodating different player preferences in how they type their commands.
        {
            if (option.validInputs != null)
            {
                bool matchFound = false;

                foreach (string keyword in option.validInputs.synonyms) // We loop through the synonyms for this option to see if any of them match the player's input. This allows for a more flexible and user-friendly command system, as players can use different words or phrases to trigger the same option, accommodating different play styles and preferences.
                {
                    if (keyword.ToLower().Trim() == input)
                    {
                        matchFound = true;
                        break;
                    }
                }

                if (matchFound) // If we found a match between the player's input and the valid inputs for this option, we proceed to execute the option. This involves checking if the option should be disabled due to being visited before (if disableIfVisited is true), and if so, whether there's an alternate node to go to instead. If the option is valid to execute, we call the method to execute it, which will handle any actions and node transitions associated with that option.
                {
                    if (option.disableIfVisited && option.nextNode != null && visitedNodes.Contains(option.nextNode))
                    {
                        if (option.alternateNodeIfBlocked != null)
                        {
                            ForceLoadNode(option.alternateNodeIfBlocked);
                            return;
                        }
                        continue;
                    }

                    ExecuteOption(option);
                    return;
                }
            }
        }

        ShowError();
    }

    void ExecuteOption(StoryOption option)
    {
        if (option.actionType != StoryAction.None) ExecuteSpecialAction(option.actionType);

        if (option.nextNode != null)
        {
            currentNode = option.nextNode;
            UpdateStoryVisuals();
            TryUpdateMap();
        }
    }

    void ExecuteSpecialAction(StoryAction action)
    {
        if (movesScript == null) return;
        switch (action)
        {
            case StoryAction.TriggerCat: movesScript.GoToCatPosition(); break;
            case StoryAction.PickAxe: movesScript.PickAxe(); break;
            case StoryAction.LookPainting: movesScript.LookPainting(); break;
            case StoryAction.Die: movesScript.GoFirstRightDie(); break;
            case StoryAction.None: default: break;
        }
    }

    void TryUpdateMap()
    {
        if (mapViewer == null) mapViewer = FindObjectOfType<MapViewer>();
        if (mapViewer != null) mapViewer.UpdateMap(currentNode);
    }

    void ShowError()
    {
        string previousText = storyLog.lastLoadedText;
        storyLog.SetTextAnimated($"<color=red>Acción no válida.</color>\n\n{previousText}");
    }

    public void ForceLoadNode(StoryNode newNode)
    {
        if (newNode == null) return;
        currentNode = newNode;
        UpdateStoryVisuals();
        TryUpdateMap();
    }

    private void OnDestroy()
    {
        foreach (Coroutine c in activeRandomSoundCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }
        activeRandomSoundCoroutines.Clear();

        foreach (AudioSource oldSource in activeAudioSources)
        {
            if (oldSource != null)
            {
                AudioFader fader = oldSource.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject();
                else
                {
                    oldSource.Stop();
                    Destroy(oldSource.gameObject);
                }
            }
        }
        activeAudioSources.Clear();

        foreach (GameObject randomObj in activeRandomSoundObjects)
        {
            if (randomObj != null)
            {
                AudioFader fader = randomObj.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject();
                else Destroy(randomObj);
            }
        }
        activeRandomSoundObjects.Clear();

        foreach (var kvp in activeAmbientSources)
        {
            if (kvp.Value != null)
            {
                AudioFader fader = kvp.Value.GetComponent<AudioFader>();
                if (fader != null) fader.FadeOutAndDestroyGameObject();
                else Destroy(kvp.Value.gameObject);
            }
        }
        activeAmbientSources.Clear();
    }

    void UpdateAISituationContext(string contextText)
    {
        if (string.IsNullOrEmpty(contextText)) return;

        BaseAIScript[] allAIs = FindObjectsOfType<BaseAIScript>(true);
        foreach (BaseAIScript ai in allAIs)
        {
            ai.SetSituationContext(contextText);
        }
    }
}