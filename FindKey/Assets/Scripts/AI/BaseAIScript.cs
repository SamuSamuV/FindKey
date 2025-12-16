using UnityEngine;

public abstract class BaseAIScript : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;
    [TextArea] public string personalityPrompt;

    public abstract void InitNPC();
}
