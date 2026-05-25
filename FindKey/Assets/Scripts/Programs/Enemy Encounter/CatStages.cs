using UnityEngine;
using UnityEngine.InputSystem; // 1. IMPORTANTE: Necesitas esta librería

public class CatAIScript_Stage1 : BaseAIScript
{
    public override void InitNPC()
    {
        npcName = "Cat";
        personalityPrompt = "You are a curious, friendly cat. You like the player.";
        systemInstruction = "Responde en español. Acción por defecto: 'none'.";
    }

    void Update()
    {
        // 2. Usamos el nuevo sistema en lugar de Input.GetKeyDown
        if (inputField != null && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame && !string.IsNullOrEmpty(inputField.text))
        {
            string message = inputField.text.ToLower();

            if (message.Contains("hola") || message.Contains("buenas") || message.Contains("saludos") || message.Contains("hey"))
            {
                Debug.Log("Gato detecta saludo, avanzando a Fase 2...");
                TransitionToStage2();
            }
        }
    }

    private void TransitionToStage2()
    {
        EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();

        if (encounterData != null)
        {
            encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage2;
        }
    }

    protected override void OpenDoor() { }
}

public class CatAIScript_Stage2 : BaseAIScript
{
    public override void InitNPC()
    {
        npcName = "Cat";
        personalityPrompt = "You are a cat that is starting to see glitches. You are confused and slightly anxious.";
        systemInstruction = "Responde en español. Acción por defecto: 'none'. Incluye palabras cortadas o errores tipo glitch.";
    }
    protected override void OpenDoor() { }
}

public class CatAIScript_Stage3 : BaseAIScript
{
    public override void InitNPC()
    {
        npcName = "CorruptedCat";
        personalityPrompt = "You are a corrupted entity inside the cat. You are aggressive and threatening.";
        systemInstruction = "Responde en español. Sé hostil. Acción por defecto: 'none'.";
    }
    protected override void OpenDoor() { }
}

public class CatAIScript_Stage4 : BaseAIScript
{
    public override void InitNPC()
    {
        npcName = "BrokenData";
        personalityPrompt = "You are a dying AI fragment. You barely have logic.";
        systemInstruction = "Responde en español. Habla con frases rotas y símbolos extraños. Acción por defecto: 'none'.";
    }
    protected override void OpenDoor() { }
}