using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class: BaseEnemyEncounter
/// Description: This script manages the base logic for an enemy encounter in the FindKey game. It handles the activation of the encounter when the player is in front of the cat,
///              including fading in the enemy visuals and configuring the AI. The script also manages the state of the encounter, ensuring that it only activates once and can be reset when necessary.
///              It interacts with other components such as AppWindow and MoveAppData to control the UI and player state during the encounter.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class BaseEnemyEncounter : MonoBehaviour
{
    public GameObject nonEnemyFindedPanel;
    public GameObject enemyVisuals;

    [Tooltip("Tiempo que tarda en aparecer el gato de la oscuridad.")]
    public float fadeDuration = 1.5f;

    public MoveAppData moveAppData;
    private AppWindow myAppWindow;
    private CanvasGroup enemyCanvasGroup;

    private bool isEncounterActive = false;

    void Awake()
    {
        InitReferences(); // Asegure that references are initialized in Awake to avoid null references in OnEnable and Start.
    }

    void OnEnable()
    {
        InitReferences(); // Ensure references are initialized in case they were not set in Awake.
        if (moveAppData != null && moveAppData.playerIsFrontCat)
        {
            CheckCatEncounter(); // Check if the encounter should be activated when the object is enabled.
        }

        else
        {
            ResetEncounter(); // If the player is not in front of the cat, ensure the encounter is reset when the object is enabled.
        }
    }

    void Start()
    {
        InitReferences(); // Ensure references are initialized in case they were not set in Awake or OnEnable.
        CheckCatEncounter(); // Check if the encounter should be activated at the start of the game.
    }

    void Update()
    {
        if (moveAppData != null && moveAppData.playerIsFrontCat && !isEncounterActive)
        {
            CheckCatEncounter(); // Continuously check if the encounter should be activated while the game is running, in case the player moves in front of the cat after the initial checks.
        }
    }

    private void InitReferences() // This method ensures that all necessary references are set, either through the Inspector or by finding them in the scene. It is called in Awake, OnEnable, and Start to guarantee that references are available when needed.
    {
        if (myAppWindow == null) myAppWindow = GetComponent<AppWindow>();
        if (moveAppData == null)
        {
            GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
            if (goMoveAppData != null) moveAppData = goMoveAppData.GetComponent<MoveAppData>();
        }

        if (enemyVisuals != null && enemyCanvasGroup == null) // Ensure that the enemy visuals have a CanvasGroup component for fading, and if not, add one.
        {
            enemyCanvasGroup = enemyVisuals.GetComponent<CanvasGroup>();
            if (enemyCanvasGroup == null) enemyCanvasGroup = enemyVisuals.AddComponent<CanvasGroup>();
        }
    }

    public void CheckCatEncounter() // This method checks if the encounter should be activated based on the player's position in front of the cat. It ensures that the encounter only activates once and manages the visual and AI setup for the encounter.
    {
        if (isEncounterActive) return;

        if (moveAppData != null && moveAppData.playerIsFrontCat)
        {
            isEncounterActive = true;

            // Activate enemy visuals and deactivate non-enemy panel, ensuring that the encounter visuals are properly set up for the player.
            if (nonEnemyFindedPanel != null) nonEnemyFindedPanel.SetActive(false);
            if (enemyVisuals != null) enemyVisuals.SetActive(true);

            // Set the current NPC type in the EnemyEncounterData to configure the AI and other encounter-related data for the cat encounter.
            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null) encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage1;

            StartCoroutine(FadeInEnemy());

            if (myAppWindow != null) myAppWindow.SetCloseAndMinimizeInteractable(false);

            DesktopManager dm = FindObjectOfType<DesktopManager>();
            if (dm != null)
            {
                foreach (var data in dm.iconsToSpawn) // Loop through the desktop icons to find the relevant one for the encounter and activate the corresponding UI elements in the Moves component, ensuring that the player has access to the necessary UI for the encounter.
                {
                    if ((data.label.Contains("FindKey") || data.label.Contains("Move")) && data.isOpen && data.windowInstance != null)
                    {
                        Moves moves = data.windowInstance.GetComponent<Moves>();
                        if (moves != null) moves.ActivateCatUI();
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator FadeInEnemy() // This coroutine handles the fade-in effect for the enemy visuals, gradually increasing the alpha of the CanvasGroup to create a smooth transition as the enemy appears on the screen during the encounter.
    {
        if (enemyCanvasGroup == null) yield break;

        enemyCanvasGroup.alpha = 0f;
        float time = 0f;

        while (time < fadeDuration) // Loop until the fade duration is reached, updating the alpha of the CanvasGroup based on the elapsed time to create the fade-in effect.
        {
            time += Time.deltaTime;
            enemyCanvasGroup.alpha = Mathf.Clamp01(time / fadeDuration);
            yield return null;
        }
        enemyCanvasGroup.alpha = 1f;
    }

    public void ResetEncounter() // This method resets the encounter state, deactivating the enemy visuals and reactivating the non-enemy panel, ensuring that the encounter can be properly reset when necessary, such as when the player moves away from the cat or when the encounter is closed.
    {
        isEncounterActive = false;
        if (nonEnemyFindedPanel != null) nonEnemyFindedPanel.SetActive(true);
        if (enemyVisuals != null) enemyVisuals.SetActive(false);
    }
}