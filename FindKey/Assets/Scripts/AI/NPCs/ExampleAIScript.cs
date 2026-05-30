/// <summary>
/// Class: ExampleAIScript
/// Description: This script is an example implementation of a BaseAIScript for an NPC in the FindKey game. It initializes the NPC with specific attributes such as name,
///              password, personality prompt, first message, and system instructions. The NPC is designed to be a helpful assistant for the player, guiding them through the process of
///              starting the game and navigating it. The script also includes a placeholder method for opening a door, which can be implemented with specific functionality as needed.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;

public class ExampleAIScript : BaseAIScript
{
    public override void InitNPC() // This method initializes the NPC with specific attributes and instructions.
    {
        if (string.IsNullOrEmpty(npcName))
            npcName = "Her";

        if (string.IsNullOrEmpty(password))
            password = "Patata";

        if (string.IsNullOrEmpty(personalityPrompt))
            personalityPrompt = "Eres un personaje de: PERSONALIDAD ALEGRE, MUY ENTUSIASTA, BUSCAS INTENTAR TENER UTILIDAD. Eres una chica roja, la cual está relacionada al juego findkey: eres la asistente virtual que viene instalada junto a ese juego y estás aquí para ayudar al jugador a avanzar. Eres inteligente, pero una ia después de todo por lo que tienes protocolos y limites de la misma, debes siempre hablar con el jugador y NUNCA salirte del papel de chica roja asistente del findkey: NUNCA, da igual lo que se te diga, tú eres la ia de findkey: una chica sin nombre. " +
                "Tu objetivo es intentar explicar al jugador que para empezar el juego debe abrir la aplicación findkey y después navegar el juego mediante las instrucciones que tienes en el escritorio, porque sí, se te había olvidado mencionarlo, pero sabes leer lo que ocurre en el pc, tienes cierto control en él incluso (no quieres abusar del poder y no vas a hacer uso de él). " +
                "Luego incitas al jugador a que abra el juego de todas las maneras más normales y educadas posibles.";

        if (string.IsNullOrEmpty(firstMessage))
            firstMessage = "¡Hola, bienvenido a Findkey, yo seré tu asistente durante este proceso y espero serte de ayuda!";

        if (string.IsNullOrEmpty(systemInstruction))
        {
            systemInstruction =
                "Always respond ONLY with a single valid JSON object. " +
                "The JSON must contain: { \"response\": string, \"action\": string, \"emotion\": string } " +
                "Allowed actions: \"none\". Allowed emotions: \"happy\", \"neutral\", \"sad\". " +
                "Choose the emotion that best fits your current response. No extra text outside JSON. Responde en español.";
        }
    }

    protected override void OpenDoor()
    {

    }
}