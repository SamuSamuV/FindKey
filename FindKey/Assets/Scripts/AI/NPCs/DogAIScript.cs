using UnityEngine;

public class DogAIScript : BaseAIScript
{
    public override void InitNPC()
    {
        if (string.IsNullOrEmpty(npcName))
            npcName = "Dog";

        if (string.IsNullOrEmpty(password))
            password = "Patata";

        if (string.IsNullOrEmpty(personalityPrompt))
            personalityPrompt = "You are a dog";

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
        Debug.Log("El perro corre en círculos emocionado y la puerta se abre.");
    }
}