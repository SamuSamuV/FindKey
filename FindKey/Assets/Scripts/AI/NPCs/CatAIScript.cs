using UnityEngine;

/// <summary>
/// Class: CatAIScript
/// Description: This script defines the behavior of the cat NPC in the FindKey game. It inherits from BaseAIScript and overrides necessary methods to set up the cat's personality,
///              initial message, and system instructions. The cat has a specific password and responds to player interactions. When the player successfully interacts with the cat
///              (e.g., "defeating" it), it triggers changes in the game state, such as marking the cat as dead, enabling certain UI elements in related windows, and forcing a transition to a
///              new story node in the adventure. The script ensures that all interactions with the cat are handled according to its defined behavior and integrates with other game systems like the
///              desktop manager and adventure manager.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class CatAIScript : BaseAIScript
{
    public StoryNode nodeAfterCat;

    void Start()
    {
        base.Start();
    }

    public override void InitNPC() // This method is called to initialize the NPC's properties and behavior. It sets default values for the cat's name, password, personality prompt, first message, and system instructions if they are not already set.
    {
        if (string.IsNullOrEmpty(npcName))
            npcName = "Cat";

        if (string.IsNullOrEmpty(password))
            password = "Patata";

        if (string.IsNullOrEmpty(personalityPrompt))
            personalityPrompt = "You are a cat";

        if (string.IsNullOrEmpty(firstMessage))
            firstMessage = "Hi human.";

        if (string.IsNullOrEmpty(systemInstruction))
        {
            systemInstruction =
                "Always respond ONLY with a single valid JSON object." +
                "The JSON must contain: { \"response\": string, \"action\": string }" +
                "Allowed actions: \"none\". No extra text outside JSON. Respond only in English.";
        }
    }

    protected override void OpenDoor() // This method is called when the player successfully interacts with the cat (e.g., "defeating" it). It updates the game state to reflect that the cat is dead, enables certain UI elements in related windows, and forces a transition to a new story node in the adventure.
    {
        moveAppData.catIsDead = true;
        Debug.Log("Gato derrotado/superado.");

        DesktopManager dm = FindObjectOfType<DesktopManager>();

        if (dm != null && dm.iconsToSpawn != null)
        {
            foreach (var data in dm.iconsToSpawn) // Loop through the icons on the desktop to find the ones related to the cat and enemy encounter, and update their UI accordingly.
            {
                if (data.label == "FindKey.exe" && data.isOpen && data.windowInstance != null)
                {
                    AppWindow findKeyWindow = data.windowInstance.GetComponent<AppWindow>();
                    if (findKeyWindow != null) findKeyWindow.SetCloseAndMinimizeInteractable(true);

                    Moves moves = data.windowInstance.GetComponent<Moves>();
                    moves.IAPanel.SetActive(false);
                    moves.MovePanel.SetActive(true);
                    moves.playerInputField.SetActive(true);

                    moves.selectMove.AddMovement(Direction.Straight);
                    moves.GoToNextStageAfterCat();
                }

                if (data.label == "Enemy Encounter" && data.isOpen && data.windowInstance != null) // This block checks for the "Enemy Encounter" window and updates its UI to reflect that the cat has been defeated, such as hiding enemy visuals and showing a panel indicating no enemy is found.
                {
                    AppWindow enemyWindow = data.windowInstance.GetComponent<AppWindow>();
                    if (enemyWindow != null) enemyWindow.SetCloseAndMinimizeInteractable(true);

                    EnemyEncounterData enemyEncounterData = data.windowInstance.GetComponent<EnemyEncounterData>();
                    BaseEnemyEncounter baseEnemyEncounter = data.windowInstance.GetComponent<BaseEnemyEncounter>();

                    baseEnemyEncounter.enemyVisuals.SetActive(false);
                    baseEnemyEncounter.nonEnemyFindedPanel.SetActive(true);

                    enemyEncounterData.CurrentType = EnemyEncounterData.NPCType.None;
                }
            }
        }

        AdventureManager adventure = FindObjectOfType<AdventureManager>();
        if (adventure != null && adventure.nodeAfterCatWin != null)
        {
            adventure.ForceLoadNode(adventure.nodeAfterCatWin);
        }
    }
}