using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    private List<Coroutine> activeRandomSoundCoroutines = new List<Coroutine>();
    private List<GameObject> activeRandomSoundObjects = new List<GameObject>();

    void Start()
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

        UpdateStoryVisuals();
        inputField.onSubmit.AddListener(ProcessInput);
        TryUpdateMap();
    }

    void UpdateStoryVisuals()
    {
        if (currentNode != null)
        {
            storyLog.SetTextAnimated(currentNode.storyText);

            if (popupSequenceCoroutine != null) StopCoroutine(popupSequenceCoroutine);

            if (currentNode.popups != null && currentNode.popups.Count > 0)
            {
                popupSequenceCoroutine = StartCoroutine(RunPopupSequence(currentNode.popups));
            }

            UpdateNodeAudio(currentNode.nodeSounds);
            UpdateRandomAudio(currentNode.randomSounds);
        }
    }

    void UpdateRandomAudio(List<RandomNodeSoundAction> randomSounds)
    {
        foreach (Coroutine c in activeRandomSoundCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }
        activeRandomSoundCoroutines.Clear();

        activeRandomSoundObjects.RemoveAll(item => item == null);

        if (randomSounds != null && randomSounds.Count > 0)
        {
            foreach (var randomSound in randomSounds)
            {
                if (randomSound.soundSettings != null && randomSound.soundSettings.IsValid())
                {
                    Coroutine newCoroutine = StartCoroutine(RandomSoundRoutine(randomSound));
                    activeRandomSoundCoroutines.Add(newCoroutine);
                }
            }
        }
    }

    IEnumerator RandomSoundRoutine(RandomNodeSoundAction randomData)
    {
        float safeMin = Mathf.Max(0f, randomData.minInterval);
        float safeMax = Mathf.Max(safeMin, randomData.maxInterval);

        float waitTime = Random.Range(safeMin, safeMax);
        if (waitTime > 0) yield return new WaitForSeconds(waitTime);

        float roll = Random.value;
        string clipName = randomData.soundSettings.clip.name;

        if (roll <= randomData.playChance)
        {
            Debug.Log($"<color=green>[Audio Aleatorio]</color> ¡ÉXITO! Tirada: {roll:F2} <= Probabilidad: {randomData.playChance:F2}. Reproduciendo: <b>{clipName}</b>");

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
        else
        {
            Debug.Log($"<color=orange>[Audio Aleatorio]</color> FALLO. Tirada: {roll:F2} > Probabilidad: {randomData.playChance:F2}. Omitiendo: <b>{clipName}</b>");
        }
    }

    void UpdateNodeAudio(List<NodeSoundAction> newSounds)
    {
        if (newSounds == null) newSounds = new List<NodeSoundAction>();

        List<AudioSource> sourcesToKeep = new List<AudioSource>();

        foreach (var action in newSounds)
        {
            if (action.soundSettings == null || !action.soundSettings.IsValid()) continue;

            AudioSource existingSource = activeAudioSources.Find(s => s != null && s.clip == action.soundSettings.clip);

            if (existingSource != null)
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
            else
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

        activeAudioSources = sourcesToKeep;
    }

    GameObject GetChannelObject(AudioChannel channel)
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

    IEnumerator RunPopupSequence(List<PopupData> popups)
    {
        foreach (PopupData data in popups)
        {
            if (data.delayBeforeSpawn > 0) yield return new WaitForSeconds(data.delayBeforeSpawn);
            SpawnSinglePopup(data);
        }
    }

    void SpawnSinglePopup(PopupData data)
    {
        GameObject prefabToUse = data.specificPrefab != null ? data.specificPrefab : defaultPopupPrefab;

        if (prefabToUse != null)
        {
            GameObject newPopup = Instantiate(prefabToUse, popupContainer);
            PopupController controller = newPopup.GetComponent<PopupController>();
            if (controller != null) controller.Setup(data);
        }
    }

    void ProcessInput(string input)
    {
        if (string.IsNullOrEmpty(input) || currentNode == null) return;

        input = input.ToLower().Trim();
        inputField.text = "";
        inputField.ActivateInputField();

        foreach (var option in currentNode.options)
        {
            if (option.validInputs != null)
            {
                foreach (string keyword in option.validInputs.synonyms)
                {
                    if (keyword.ToLower().Trim() == input)
                    {
                        ExecuteOption(option);
                        return;
                    }
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
        storyLog.SetTextAnimated($"<color=red>You can't do that here.</color>\n\n{previousText}");
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
    }
}