using System;
using UnityEngine;

public class EnemyEncounterData : MonoBehaviour
{
    public MoveAppData moveAppData;
    public StoryLog myAppStoryLog;

    [Header("Configuración de Fases del Gato")]
    public NPCProfile catStage1Profile;
    public NPCProfile catStage2Profile;
    public NPCProfile catStage3Profile;
    public NPCProfile catStage4Profile;

    [Header("Cinturón de Seguridad (Fase 2)")]
    public string[] adjetivosDeSeguridadFase2 = new string[] {
        "feo", "tonto", "pesado", "malo", "raro", "bonito", "lindo", "adorable", "estúpido", "listo",
        "bueno", "agradable", "genial", "horrible", "asqueroso", "mono", "molesto", "extraño", "creepy"
    };

    [Header("Efectos Fase 3 (Corrupción)")]
    public AudioClip zumbidoClip;
    public AudioClip transicionClip;
    public AudioClip fondoCorruptoClip;
    public AudioClip[] sonidosRandomCorruptos;

    // --- NUEVO: Huecos para los sprites de la transformación ---
    [Header("Animaciones Fase 3 (Sprites)")]
    public Sprite[] transformSprites;
    public Sprite[] corruptedIdleSprites;
    public Sprite[] corruptedBlinkSprites;
    public Sprite[] corruptedTalkingSprites;

    [Header("Datos Guardados de la Historia")]
    public string respuestaIdentidad = ""; // Aquí guardaremos la respuesta a "¿Sabes quién soy?"

    public enum NPCType { None, CatStage1, CatStage2, CatStage3, CatStage4 }

    [SerializeField]
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

    void Awake() { InitCheck(); }
    void OnEnable() { InitCheck(); }
    void Start() { InitCheck(); }

    void Update()
    {
        if (moveAppData != null && moveAppData.playerIsFrontCat && selectedType == NPCType.None)
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

        if (moveAppData != null && moveAppData.playerIsFrontCat && selectedType == NPCType.None)
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
                SetupAIReferences(c1, catStage1Profile);
                currentAI = c1;
                break;

            case NPCType.CatStage2:
                var c2 = gameObject.AddComponent<CatAIScript_Stage2>();
                SetupAIReferences(c2, catStage2Profile);
                c2.isProactiveTriggered = true;
                c2.adjetivosDeSeguridad = adjetivosDeSeguridadFase2;
                currentAI = c2;
                break;

            case NPCType.CatStage3:
                var c3 = gameObject.AddComponent<CatAIScript_Stage3>();
                SetupAIReferences(c3, catStage3Profile);
                c3.isProactiveTriggered = true;

                // Inyectamos los audios a la Fase 3
                c3.zumbidoClip = zumbidoClip;
                c3.transicionClip = transicionClip;
                c3.fondoCorruptoClip = fondoCorruptoClip;
                c3.sonidosRandomCorruptos = sonidosRandomCorruptos;

                // --- NUEVO: Inyectamos los sprites a la Fase 3 ---
                c3.transformSprites = transformSprites;
                c3.corruptedIdleSprites = corruptedIdleSprites;
                c3.corruptedBlinkSprites = corruptedBlinkSprites;
                c3.corruptedTalkingSprites = corruptedTalkingSprites;

                currentAI = c3;
                break;

            case NPCType.CatStage4:
                var c4 = gameObject.AddComponent<CatAIScript_Stage4>();
                SetupAIReferences(c4, catStage4Profile);
                c4.isProactiveTriggered = true;
                currentAI = c4;
                break;
        }
    }

    private void SetupAIReferences(BaseAIScript newAI, NPCProfile profile)
    {
        if (profile != null) newAI.LoadProfile(profile);

        newAI.visualController = GetComponentInChildren<NPCVisualController>(true);

        // --- NUEVO: Si estamos en Fase 1, restauramos los sprites del gato original por seguridad ---
        if (selectedType == NPCType.CatStage1 && newAI.visualController != null)
        {
            newAI.visualController.RestoreDefaultSprites();
        }

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