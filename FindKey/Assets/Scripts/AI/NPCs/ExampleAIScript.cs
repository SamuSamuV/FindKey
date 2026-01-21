using UnityEngine;

public class ExampleAIScript : BaseAIScript
{
    public override void InitNPC()
    {
        if (string.IsNullOrEmpty(npcName))
            npcName = "Her";

        if (string.IsNullOrEmpty(password))
            password = "Patata";

        if (string.IsNullOrEmpty(personalityPrompt))
            personalityPrompt = "You are an egg";

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

    }
}