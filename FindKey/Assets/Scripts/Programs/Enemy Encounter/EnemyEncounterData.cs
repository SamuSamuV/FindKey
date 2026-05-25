using System;
using UnityEngine;

public class EnemyEncounterData : MonoBehaviour
{
    public MoveAppData moveAppData;
    public StoryLog myAppStoryLog;

    public NPCProfile catProfile;
    public NPCProfile dogProfile;

    public enum NPCType { None, Cat, Dog }

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

    // --- EL GUARDIÁN ABSOLUTO ---
    // Da igual cuándo se abra la app, si estamos frente al gato, se fuerza el tipo Gato.
    void Update()
    {
        if (moveAppData != null && moveAppData.playerIsFrontCat && selectedType != NPCType.Cat)
        {
            CurrentType = NPCType.Cat;
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
            CurrentType = NPCType.Cat;
        }
        else if (currentAI == null && selectedType != NPCType.None)
        {
            ApplyAI();
        }
    }

    // ¡HEMOS BORRADO ONVALIDATE() PARA EVITAR QUE UNITY SOBREESCRIBA EL TIPO A "NONE"!

    private void ApplyAI()
    {
        if (currentAI != null) Destroy(currentAI);

        switch (selectedType)
        {
            case NPCType.Cat:
                var cat = gameObject.AddComponent<CatAIScript>();
                SetupAIReferences(cat, catProfile);
                currentAI = cat;
                break;

            case NPCType.Dog:
                var dog = gameObject.AddComponent<DogAIScript>();
                SetupAIReferences(dog, dogProfile);
                currentAI = dog;
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