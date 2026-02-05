using UnityEngine;

public class CatAIScript : BaseAIScript
{
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

        DesktopManager dm = FindObjectOfType<DesktopManager>();

        if (dm != null && dm.iconsToSpawn != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == "Move")
                {
                    if (data.isOpen && data.windowInstance != null)
                    {

                        //////////////////////////////////////////////////////

                        //Implementar animación del FadeOut del gato proximamente con una corrutina y hacer todo lo de ocultar los paneles y demas al final de la corrutina/animación al irse

                        //////////////////////////////////////////////////////

                        Moves moves = data.windowInstance.GetComponent<Moves>();

                        moves.IAPanel.SetActive(false);
                        moves.MovePanel.SetActive(true);
                        moves.playerInputField.SetActive(true);

                        moves.selectMove.AddMovement(Direction.Straight);
                        moves.GoToNextStageAfterCat();
                    }
                }

                else if (data.label == "Enemy Encounter")
                {
                    if (data.isOpen && data.windowInstance != null)
                    {
                        EnemyEncounterData enemyEncounterData = GetComponent<EnemyEncounterData>();
                        BaseEnemyEncounter baseEnemyEncounter = data.windowInstance.GetComponent<BaseEnemyEncounter>();

                        baseEnemyEncounter.nonEnemyFindedPanel.SetActive(true);

                        enemyEncounterData.CurrentType = EnemyEncounterData.NPCType.None;
                    }
                }
            }
        }
    }
}