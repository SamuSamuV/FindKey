using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Moves : MonoBehaviour
{
    [Header("Location Texts")]
    [TextArea(3, 6)]
    public string startText;

    [TextArea(3, 6)]
    public string youComeBackToStartText;

    [TextArea(3, 6)]
    public string rightPathDieText;

    [TextArea(3, 6)]
    public string goToPaintingText;

    [TextArea(3, 6)]
    public string lookPaintingText;

    [TextArea(3, 6)]
    public string hasAlreadyLookPaintingText;

    [TextArea(3, 6)]
    public string goFirstStraightText;

    [TextArea(3, 6)]
    public string goFirstStraightButYouReturnFromTheAxeText;

    [TextArea(3, 6)]
    public string goFirstStraightButYouReturnFromTheCatPositionText;

    [TextArea(3, 6)]
    public string nextStageNextToCatText;

    [TextArea(3, 6)]
    public string goToAxeText;

    [TextArea(3, 6)]
    public string goToAxeButItWasAlreadyPickedText;

    [TextArea(3, 6)]
    public string pickAxeText;

    [TextArea(3, 6)]
    public string hasAlreadyPickAxeText;

    [TextArea(3, 6)]
    public string goToAliveCatText;

    [TextArea(3, 6)]
    public string goToDeadCatText;

    [TextArea(3, 6)]
    public string catKillsYouWhenYouTryToRunText;

    [TextArea(3, 6)]
    public string catKillsYouWhenYouTryToKillItWithOutAxeText;

    [TextArea(3, 6)]
    public string youKillTheCatText;


    public StoryLog storyLog;
    public SelectMove selectMove;
    public MoveAppManager moveAppManager;
    public MoveAppData moveAppData;
    public InventoryManager inventoryManager;
    public DesktopManager desktopManager;
    [SerializeField] public GameObject playerInputField;

    void Awake()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
            moveAppData = goMoveAppData.GetComponent<MoveAppData>();
    }

    void Start()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        moveAppData = goMoveAppData.GetComponent<MoveAppData>();
        if (storyLog != null) storyLog.SetTextAnimated(startText);
    }

    public void GoFirstRightDie()
    {
        moveAppManager.dead = true;
        playerInputField.SetActive(false);
        storyLog.SetTextAnimated(rightPathDieText);
        Debug.Log("Has muerto");
    }

    public void CatKillYouWhenYouRun()
    {
        moveAppManager.dead = true;
        playerInputField.SetActive(false);
        storyLog.SetTextAnimated(catKillsYouWhenYouTryToRunText);
        Debug.Log("Has muerto");
    }

    public void GoToPainting()
    {
        storyLog.SetTextAnimated(goToPaintingText);
    }

    public void LookPainting()
    {
        if (!moveAppData.playerHasAlreadySeeThis)
        {
            storyLog.SetTextAnimated(lookPaintingText);
            moveAppData.playerHasAlreadySeeThis = true;
        }

        else
        {
            storyLog.SetTextAnimated(hasAlreadyLookPaintingText);
        }
    }

    public void GoToAxe()
    {
        if (!moveAppData.hasAxe)
            storyLog.SetTextAnimated(goToAxeText);

        else
            storyLog.SetTextAnimated(goToAxeButItWasAlreadyPickedText);
    }

    public void PickAxe()
    {
        if (!moveAppData.hasAxe)
        {
            storyLog.SetTextAnimated(pickAxeText);
            moveAppData.hasAxe = true;

            DesktopManager dm = FindObjectOfType<DesktopManager>();
            
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == "Inventory")
                {
                    if (data.isOpen)
                    {
                        InventoryManager inventoryManager = data.windowInstance.GetComponent<InventoryManager>();

                        if (inventoryManager != null)
                        {
                            inventoryManager.AddAxeToInventary();
                        }
                    }
                    break;
                }
            }
        }
        else
        {
            storyLog.SetTextAnimated(hasAlreadyPickAxeText);
        }
    }

    public void FirstGoStraight()
    {
        storyLog.SetTextAnimated(goFirstStraightText);
    }

    public void GoToCatPosition()
    {
        // Protección: Si moveAppData es null, salimos para no crashear
        if (moveAppData == null) return;

        if (!moveAppData.catIsDead)
        {
            moveAppData.playerIsFrontCat = true;
            if (playerInputField) playerInputField.SetActive(false);
            storyLog.SetTextAnimated(goToAliveCatText);

            DesktopManager dm = FindObjectOfType<DesktopManager>();

            if (dm != null && dm.iconsToSpawn != null)
            {
                foreach (var data in dm.iconsToSpawn)
                {
                    if (data.label == "Enemy Encounter")
                    {
                        if (data.isOpen && data.windowInstance != null)
                        {
                            EnemyEncounterData enemyEncounterData = data.windowInstance.GetComponent<EnemyEncounterData>();

                            if (enemyEncounterData != null)
                            {
                                enemyEncounterData.CurrentType = EnemyEncounterData.NPCType.Cat;
                                if (enemyEncounterData.nonEnemyFindedPanel)
                                    enemyEncounterData.nonEnemyFindedPanel.SetActive(false);
                            }
                        }
                        break;
                    }
                }
            }

            else
            {
                Debug.LogWarning("Moves: No se encontró el DesktopManager o sus iconos. Ignorando lógica de ventana externa.");
            }
        }

        else
        {
            moveAppData.playerIsFrontCat = false;
            storyLog.SetTextAnimated(goToDeadCatText);
            if (playerInputField) playerInputField.SetActive(true);
        }
    }

    public void GoToNextStageAfterCat()
    {
        moveAppData.playerIsFrontCat = false;
        storyLog.SetTextAnimated(nextStageNextToCatText);
    }
}

    //public void AtackCat()
    //{
    //    if (!moveAppData.hasAxe)
    //    {
    //        moveAppManager.dead = true;
    //        playerInputField.SetActive(false);
    //        storyLog.SetText(catKillsYouWhenYouTryToKillItWithOutAxeText);
    //        Debug.Log("Has muerto");
    //    }

    //    else
    //    {
    //        storyLog.SetText(youKillTheCatText);
    //        moveAppData.catIsDead = true;
    //    }
    //}
