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
    public string goToAxeText;

    [TextArea(3, 6)]
    public string goToAxeButItWasAlreadyPickedText;

    [TextArea(3, 6)]
    public string pickAxeText;

    [TextArea(3, 6)]
    public string hasAlreadyPickAxeText;

    [TextArea(3, 6)]
    public string goToCatText;

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
        storyLog.AddLine(rightPathDieText);
        Debug.Log("Has muerto");
    }

    public void CatKillYouWhenYouRun()
    {
        moveAppManager.dead = true;
        playerInputField.SetActive(false);
        storyLog.AddLine(catKillsYouWhenYouTryToRunText);
        Debug.Log("Has muerto");
    }

    public void GoToPainting()
    {
        storyLog.AddLine(goToPaintingText);
    }

    public void LookPainting()
    {
        if (!moveAppData.playerHasAlreadySeeThis)
        {
            storyLog.AddLine(lookPaintingText);
            moveAppData.playerHasAlreadySeeThis = true;
        }

        else
        {
            storyLog.AddLine(hasAlreadyLookPaintingText);
        }
    }

    public void GoToAxe()
    {
        if (!moveAppData.hasAxe)
            storyLog.AddLine(goToAxeText);

        else
            storyLog.AddLine(goToAxeButItWasAlreadyPickedText);
    }

    public void PickAxe()
    {
        if (!moveAppData.hasAxe)
        {
            storyLog.AddLine(pickAxeText);
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
            storyLog.AddLine(hasAlreadyPickAxeText);
        }
    }

    public void FirstGoStraight()
    {
        storyLog.AddLine(goFirstStraightText);
    }

    public void GoToCat()
    {
        storyLog.AddLine(goToCatText);
    }

    public void AtackCat()
    {
        if (!moveAppData.hasAxe)
        {
            moveAppManager.dead = true;
            playerInputField.SetActive(false);
            storyLog.AddLine(catKillsYouWhenYouTryToKillItWithOutAxeText);
            Debug.Log("Has muerto");
        }

        else
        {
            storyLog.AddLine(youKillTheCatText);
            moveAppData.catIsDead = true;
        }
    }
}