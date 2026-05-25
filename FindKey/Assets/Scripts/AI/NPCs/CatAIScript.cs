using UnityEngine;

public class CatAIScript : BaseAIScript
{
    public StoryNode nodeAfterCat;

    void Start()
    {
        base.Start();
    }

    public override void InitNPC()
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

    protected override void OpenDoor()
    {
        moveAppData.catIsDead = true;
        Debug.Log("Gato derrotado/superado.");

        DesktopManager dm = FindObjectOfType<DesktopManager>();

        if (dm != null && dm.iconsToSpawn != null)
        {
            foreach (var data in dm.iconsToSpawn)
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

                if (data.label == "Enemy Encounter" && data.isOpen && data.windowInstance != null)
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