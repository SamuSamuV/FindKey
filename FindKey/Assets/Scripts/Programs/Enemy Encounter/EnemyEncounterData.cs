using System;
using UnityEngine;

public class EnemyEncounterData : MonoBehaviour
{
    public MoveAppData moveAppData;
    public StoryLog myAppStoryLog;

    public NPCProfile catProfile;
    public NPCProfile dogProfile;

    public enum NPCType { None, Dog, CatStage1, CatStage2, CatStage3, CatStage4 }

    [SerializeField]
    [Tooltip("Cambia esto en tiempo real y la IA cambiará sola.")]
    private NPCType selectedType = NPCType.None;

    public NPCType CurrentType
    {
        get { return selectedType; }
        set
        {
            if (selectedType != value || currentAI == null)
            {
                selectedType = value;
                if (selectedType != NPCType.None)
                    ApplyAI();
            }
        }
    }

    private BaseAIScript currentAI;

    void Awake()
    {
        InitCheck();
    }

    void OnEnable()
    {
        InitCheck();
    }

    void Start()
    {
        InitCheck();
    }

    void Update()
    {
        if (moveAppData != null && moveAppData.playerIsFrontCat && selectedType != NPCType.CatStage1)
        {
            CurrentType = NPCType.CatStage1;
        }
    }

    private void InitCheck()
    {
        if (moveAppData == null)
        {
            GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
            if (goMoveAppData != null) moveAppData = goMoveAppData.GetComponent<MoveAppData>();
        }

        if (moveAppData != null && moveAppData.playerIsFrontCat)
        {
            CurrentType = NPCType.CatStage1;
        }
        else if (currentAI == null && selectedType != NPCType.None)
        {
            ApplyAI();
        }
    }

    private void ApplyAI()
    {
        if (currentAI != null) Destroy(currentAI);

        switch (selectedType)
        {
            case NPCType.CatStage1:
                var c1 = gameObject.AddComponent<CatAIScript_Stage1>();
                SetupAIReferences(c1, catProfile);
                currentAI = c1;
                break;
            case NPCType.CatStage2:
                var c2 = gameObject.AddComponent<CatAIScript_Stage2>();
                SetupAIReferences(c2, catProfile);
                currentAI = c2;
                break;
            case NPCType.CatStage3:
                var c3 = gameObject.AddComponent<CatAIScript_Stage3>();
                SetupAIReferences(c3, catProfile);
                currentAI = c3;
                break;
            case NPCType.CatStage4:
                var c4 = gameObject.AddComponent<CatAIScript_Stage4>();
                SetupAIReferences(c4, catProfile);
                currentAI = c4;
                break;
        }
    }

    private void SetupAIReferences(BaseAIScript newAI, NPCProfile profile)
    {
        newAI.LoadProfile(profile);
        newAI.visualController = GetComponentInChildren<NPCVisualController>(true);

        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if ((data.label.Contains("FindKey") || data.label.Contains("Move")) && data.isOpen && data.windowInstance != null)
                {
                    AI_References refs = data.windowInstance.GetComponentInChildren<AI_References>(true);
                    if (refs != null)
                    {
                        newAI.inputField = refs.inputField;
                        newAI.chatOutput = refs.chatOutput;
                        newAI.ollamaClient = refs.ollamaClient;
                        newAI.storyLog = refs.storyLog;
                        newAI.thinkingPanel = refs.thinkingPanel;
                    }
                    break;
                }
            }
        }

        if (newAI.ollamaClient == null) newAI.ollamaClient = FindObjectOfType<OllamaClient>(true);
        if (newAI.storyLog == null) newAI.storyLog = FindObjectOfType<StoryLog>(true);
    }

    public void ResetNPC()
    {
        DesktopManager dm = FindObjectOfType<DesktopManager>();

        if (dm != null && dm.iconsToSpawn != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if ((data.label == "Buscador Enemigos" || data.label == "Enemy Encounter") && data.isOpen && data.windowInstance != null)
                {
                    BaseEnemyEncounter baseEnemyEncounter = data.windowInstance.GetComponent<BaseEnemyEncounter>();
                    if (baseEnemyEncounter != null) baseEnemyEncounter.nonEnemyFindedPanel.SetActive(true);
                    break;
                }
            }
        }

        if (currentAI != null) Destroy(currentAI);
        currentAI = null;
        selectedType = NPCType.None;
    }

    public void AdvanceCatStage()
    {
        EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();

        if (encounterData.CurrentType == EnemyEncounterData.NPCType.CatStage1)
            encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage2;

        else if (encounterData.CurrentType == EnemyEncounterData.NPCType.CatStage2)
            encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage3;

        else if (encounterData.CurrentType == EnemyEncounterData.NPCType.CatStage3)
            encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage4;
    }
}

[Serializable]
public class NPCProfile
{
    public string npcName;
    [Tooltip("Déjalo vacío si es pacífico")]
    public string password;

    [TextArea(4, 10)]
    public string personalityPrompt;

    [TextArea(2, 5)]
    public string firstMessage;

    [TextArea(4, 10)]
    public string systemInstruction;
}