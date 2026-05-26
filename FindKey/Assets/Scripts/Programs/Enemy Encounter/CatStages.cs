using UnityEngine;

// ==========================================
// FASE 1: EL GATO AMIGABLE
// ==========================================
public class CatAIScript_Stage1 : BaseAIScript
{
    public override void InitNPC() { } // Se configura desde el Inspector

    protected override void OnAIResponse(string raw)
    {
        // 1. Guardamos lo que dijo el jugador ANTES de que el script base lo borre por seguridad
        string currentPlayerText = lastPlayerText;

        // 2. Dejamos que el script base haga su trabajo normal (escribir, animar, etc.)
        base.OnAIResponse(raw);

        // 3. CANDADO ANTI-CORTOCIRCUITO:
        // Si el jugador no ha escrito nada (es decir, la IA está enviando su primer mensaje de saludo),
        // es IMPOSIBLE avanzar de fase. Ignoramos cualquier locura que haya puesto la IA en el JSON.
        if (string.IsNullOrEmpty(currentPlayerText))
        {
            return;
        }

        // 4. Evaluación Híbrida (Solo se ejecuta si el jugador realmente ha hablado)
        string lowerRaw = raw.ToLowerInvariant();
        string lowerPlayer = currentPlayerText.ToLowerInvariant();

        // ¿La IA puso la acción en el JSON?
        bool aiDecided = lowerRaw.Contains("\"action\":\"next_stage\"") || lowerRaw.Contains("\"action\": \"next_stage\"");

        // ¿O el jugador dijo una palabra clave? (Red de seguridad)
        bool playerGreeted = lowerPlayer.Contains("hola") || lowerPlayer.Contains("buenas") ||
                             lowerPlayer.Contains("saludos") || lowerPlayer.Contains("hey") ||
                             lowerPlayer.Contains("miau");

        if (aiDecided || playerGreeted)
        {
            Debug.Log("<color=magenta>[IA GATO]</color> Saludo detectado tras hablar el jugador. Avanzando a Fase 2 en secreto...");

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null)
            {
                encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage2;
            }
        }
    }

    protected override void OpenDoor() { }
}

// ==========================================
// FASE 2: EL GATO CONFUNDIDO / GLITCHEADO
// ==========================================
public class CatAIScript_Stage2 : BaseAIScript
{
    public override void InitNPC() { }
    protected override void OpenDoor() { }
}

// ==========================================
// FASE 3: LA ENTIDAD HOSTIL / CORRUPTA
// ==========================================
public class CatAIScript_Stage3 : BaseAIScript
{
    public override void InitNPC() { }
    protected override void OpenDoor() { }
}

// ==========================================
// FASE 4: EL SISTEMA ROTO / DATOS DAÑADOS
// ==========================================
public class CatAIScript_Stage4 : BaseAIScript
{
    public override void InitNPC() { }
    protected override void OpenDoor() { }
}