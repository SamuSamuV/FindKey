using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
    [SerializeField] GameObject playerInputField;

    void Start()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        moveAppData = goMoveAppData.GetComponent<MoveAppData>();
    }

    public void GoFirstRightDie()
    {
        moveAppManager.dead = true;
        playerInputField.SetActive(false);
        storyLog.SetText(rightPathDieText);
        Debug.Log("Has muerto");
    }

    public void CatKillYouWhenYouRun()
    {
        moveAppManager.dead = true;
        playerInputField.SetActive(false);
        storyLog.SetText(catKillsYouWhenYouTryToRunText);
        Debug.Log("Has muerto");
    }

    public void GoToPainting()
    {
        storyLog.SetText(goToPaintingText);
    }

    public void LookPainting()
    {
        if (!moveAppData.playerHasAlreadySeeThis)
        {
            storyLog.SetText(lookPaintingText);
            moveAppData.playerHasAlreadySeeThis = true;
        }

        else
        {
            storyLog.SetText(hasAlreadyLookPaintingText);
        }
    }

    public void GoToAxe()
    {
        if (!moveAppData.hasAxe)
            storyLog.SetText(goToAxeText);

        else
            storyLog.SetText(goToAxeButItWasAlreadyPickedText);
    }

    public void PickAxe()
    {
        if (!moveAppData.hasAxe)
        {
            storyLog.SetText(pickAxeText);
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
            storyLog.SetText(hasAlreadyPickAxeText);
        }
    }

    public void FirstGoStraight()
    {
        storyLog.SetText(goFirstStraightText);
    }

    public void GoToCatPosition()
    {
        if (!moveAppData.catIsDead)
        {
            moveAppData.playerIsFrontCat = true;
            playerInputField.SetActive(false);
            storyLog.SetText(goToAliveCatText);

            DesktopManager dm = FindObjectOfType<DesktopManager>();

            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == "Enemy Encounter")
                {
                    if (data.isOpen)
                    {
                        EnemyEncounterData enemyEncounterData = data.windowInstance.GetComponent<EnemyEncounterData>();
                        enemyEncounterData.CurrentType = EnemyEncounterData.NPCType.Cat;

                        if (enemyEncounterData != null)
                        {
                            enemyEncounterData.nonEnemyFindedPanel.SetActive(false);
                        }
                    }
                    break;
                }
            }
        }

        else
        {
            storyLog.SetText(goToDeadCatText);
        }
    }

    public void GoToNextStageAfterCat()
    {
        storyLog.SetText(nextStageNextToCatText);
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
}