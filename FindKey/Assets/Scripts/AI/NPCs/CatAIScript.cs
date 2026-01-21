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
        Debug.Log("El gato empuja la puerta con la patita con desdén.");
    }
}