using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        InitReferences();
    }

    void OnEnable()
    {
        InitReferences();
        CheckCatEncounter();
    }

    void Start()
    {
        InitReferences();
        CheckCatEncounter();
    }

    // --- EL GUARDIÁN VISUAL ---
    void Update()
    {
        if (moveAppData != null && moveAppData.playerIsFrontCat && !isEncounterActive)
        {
            CheckCatEncounter();
        }
    }

    private void InitReferences()
    {
        if (myAppWindow == null) myAppWindow = GetComponent<AppWindow>();
        if (moveAppData == null)
        {
            GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
            if (goMoveAppData != null) moveAppData = goMoveAppData.GetComponent<MoveAppData>();
        }

        if (enemyVisuals != null && enemyCanvasGroup == null)
        {
            enemyCanvasGroup = enemyVisuals.GetComponent<CanvasGroup>();
            if (enemyCanvasGroup == null) enemyCanvasGroup = enemyVisuals.AddComponent<CanvasGroup>();
        }
    }

    public void CheckCatEncounter()
    {
        if (isEncounterActive) return;

        if (moveAppData != null && moveAppData.playerIsFrontCat)
        {
            isEncounterActive = true;

            EnemyEncounterData encounterData = GetComponent<EnemyEncounterData>();
            if (encounterData != null) encounterData.CurrentType = EnemyEncounterData.NPCType.CatStage1;

            nonEnemyFindedPanel.SetActive(false);
            enemyVisuals.SetActive(true);

            StartCoroutine(FadeInEnemy());

            if (myAppWindow != null) myAppWindow.SetCloseAndMinimizeInteractable(false);

            DesktopManager dm = FindObjectOfType<DesktopManager>();
            if (dm != null)
            {
                foreach (var data in dm.iconsToSpawn)
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

    private IEnumerator FadeInEnemy()
    {
        if (enemyCanvasGroup == null) yield break;

        enemyCanvasGroup.alpha = 0f;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            enemyCanvasGroup.alpha = Mathf.Clamp01(time / fadeDuration);
            yield return null;
        }
        enemyCanvasGroup.alpha = 1f;
    }

    public void ResetEncounter()
    {
        isEncounterActive = false;
        if (nonEnemyFindedPanel != null) nonEnemyFindedPanel.SetActive(true);
        if (enemyVisuals != null) enemyVisuals.SetActive(false);
    }
}