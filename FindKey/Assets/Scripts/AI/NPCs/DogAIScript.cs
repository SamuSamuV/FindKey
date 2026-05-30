using UnityEngine;

/// <summary>
/// Class: DogAIScript
/// Description: This script defines the behavior of a dog NPC in the FindKey game. It inherits from BaseAIScript and overrides the InitNPC method to set specific values for the
///              dog's name, password, personality prompt, first message, and system instruction. The OpenDoor method is also overridden to provide a unique response when the dog opens a door,
///              describing the dog's excited behavior as it runs in circles and opens the door. This script allows the dog NPC to interact with the player in a way that fits its character and adds
///              flavor to the game.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class DogAIScript : BaseAIScript
{
    public override void InitNPC() // Override the InitNPC method to set specific values for the dog NPC
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