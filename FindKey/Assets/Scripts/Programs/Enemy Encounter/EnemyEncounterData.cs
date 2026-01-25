using System;
using System.Xml.Linq;
using UnityEngine;

public class EnemyEncounterData : MonoBehaviour
{
    public MoveAppData moveAppData;
    public GameObject nonEnemyFindedPanel;
    public StoryLog myAppStoryLog;

    // Tus perfiles de datos
    public NPCProfile catProfile;
    public NPCProfile dogProfile;

    public enum NPCType { None, Cat, Dog }

    [SerializeField]
    [Tooltip("Cambia esto en tiempo real y la IA cambiará sola.")]
    private NPCType selectedType;

    public NPCType CurrentType
    {
        get { return selectedType; }
        set
        {
            if (selectedType != value)
            {
                selectedType = value;
                ApplyAI();
            }
        }
    }
    private BaseAIScript currentAI;

    void Start()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        moveAppData = goMoveAppData.GetComponent<MoveAppData>();

        if (moveAppData.playerIsFrontCat)
        {
            CurrentType = NPCType.Cat;
            nonEnemyFindedPanel.SetActive(false);
        }

        if (currentAI == null)
        {
            ApplyAI();
        }
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyAI();
        }
    }

    private void ApplyAI()
    {
        switch (selectedType)
        {
            case NPCType.Cat:
                var cat = gameObject.AddComponent<CatAIScript>();
                cat.storyLog = myAppStoryLog;
                cat.LoadProfile(catProfile);
                currentAI = cat;
                break;

            case NPCType.Dog:
                var dog = gameObject.AddComponent<DogAIScript>();
                dog.storyLog = myAppStoryLog;
                dog.LoadProfile(dogProfile);
                currentAI = dog;
                break;
        }
    }

    public void ResetNPC()
    {
        CurrentType = NPCType.None;
        nonEnemyFindedPanel.SetActive(true);
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