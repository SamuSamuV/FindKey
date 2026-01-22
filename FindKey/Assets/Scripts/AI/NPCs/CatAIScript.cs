using UnityEngine;

public class CatAIScript : BaseAIScript
{
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
        DesktopManager dm = FindObjectOfType<DesktopManager>();

        foreach (var data in dm.iconsToSpawn)
        {
            if (data.label == "Move")
            {
                if (data.isOpen)
                {
                    Moves moves = data.windowInstance.GetComponent<Moves>();

                    if (moves != null)
                    {
                        moves.playerInputField.SetActive(true);
                        moves.selectMove.AddMovement(Direction.Straight);
                        moves.GoToNextStageAfterCat();
                    }
                }

                break;
            }

            if (data.label == "Enemy Encounter")
            {
                if (data.isOpen)
                {
                    EnemyEncounterData enemyEncounterData = data.windowInstance.GetComponent<EnemyEncounterData>();

                    if (enemyEncounterData != null)
                    {
                        //Implementar animación del FadeOut del gato proximamente y en cto la acabe volver ha hacer el panel de no hay enemigo true

                        enemyEncounterData.nonEnemyFindedPanel.SetActive(true);

                        //Al final del todo:
                        enemyEncounterData.CurrentType = EnemyEncounterData.NPCType.None;
                    }
                }

                break;
            }
        }
    }
}