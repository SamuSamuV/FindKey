/// <summary>
/// Class: Moves
/// Description: This script manages the different moves and interactions that the player can perform in the movement map of the FindKey game. It contains methods for
///              handling various actions such as going to specific locations, interacting with objects, and dealing with encounters. The script also manages the state
///              of the game related to these moves, such as whether the player has picked up certain items or if they have died. It interacts with other components like StoryLog,
///              InventoryManager, and DesktopManager to update the game's narrative and inventory based on the player's actions.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

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
    [SerializeField] public GameObject IAPanel;
    [SerializeField] public GameObject MovePanel;

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

        IAPanel.SetActive(false);
        MovePanel.SetActive(true);

        //MapViewer mapViewer = FindObjectOfType<MapViewer>();

        //if (mapViewer != null)
        //{
        //    mapViewer.SetManager(this.moveAppManager);
        //}
    }

    public void GoFirstRightDie()
    {
        moveAppManager.dead = true;
        playerInputField.SetActive(false);
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
                if (data.label == "Inventario")
                {
                    if (data.isOpen)
                    {
                        InventoryManager inventoryManager = data.windowInstance.GetComponent<InventoryManager>();

                        if (inventoryManager != null)
                        {
                            //inventoryManager.AddAxeToInventary();
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

    public void PickUpCorruptedChest()
    {
        if (!moveAppData.hasChest)
        {
            moveAppData.hasChest = true;

            DesktopManager dm = FindObjectOfType<DesktopManager>();

            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == "Inventario")
                {
                    if (data.isOpen)
                    {
                        Debug.Log("Agregando cofre corrupto al inventario...");
                        InventoryManager inventoryManager = data.windowInstance.GetComponent<InventoryManager>();

                        if (inventoryManager != null)
                        {
                            //inventoryManager.AddCorruptedChestToInventary();
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

    // --- NUEVA FUNCIÓN SECUNDARIA: Activa la UI sin llamar a la otra app (Evita bucles) ---
    public void ActivateCatUI()
    {
        IAPanel.SetActive(true);
        MovePanel.SetActive(false);

        AppWindow myApp = GetComponent<AppWindow>();
        if (myApp != null) myApp.SetCloseAndMinimizeInteractable(false);
    }

    public void GoToCatPosition()
    {
        if (moveAppData == null) return;

        if (!moveAppData.catIsDead)
        {
            moveAppData.playerIsFrontCat = true;
            DesktopManager dm = FindObjectOfType<DesktopManager>();
            bool enemyAppOpen = false;

            if (dm != null && dm.iconsToSpawn != null)
            {
                foreach (var data in dm.iconsToSpawn)
                {
                    if (data.label == "Enemy Encounter")
                    {
                        if (data.isOpen && data.windowInstance != null)
                        {
                            enemyAppOpen = true;

                            // IMPORTANTE: Como EnemyEncounterData está en la app Enemy Encounter,
                            // lo extraemos de su windowInstance.
                            EnemyEncounterData enemyEncounterData = data.windowInstance.GetComponent<EnemyEncounterData>();
                            BaseEnemyEncounter baseEnemyEncounter = data.windowInstance.GetComponent<BaseEnemyEncounter>();

                            if (enemyEncounterData != null)
                                enemyEncounterData.CurrentType = EnemyEncounterData.NPCType.CatStage1;

                            ActivateCatUI();

                            if (baseEnemyEncounter != null)
                                baseEnemyEncounter.CheckCatEncounter();
                        }
                        break;
                    }
                }
            }

            if (!enemyAppOpen)
            {
                IAPanel.SetActive(false);
                MovePanel.SetActive(true);
                if (playerInputField) playerInputField.SetActive(false);
            }
        }
        else
        {
            IAPanel.SetActive(false);
            MovePanel.SetActive(true);
            moveAppData.playerIsFrontCat = false;
            if (playerInputField) playerInputField.SetActive(true);
        }
    }

    public void GoToNextStageAfterCat()
    {
        moveAppData.playerIsFrontCat = false;
        // YA NO HAY TEXTO HARDCODEADO. El AdventureManager leerá el nodo 'After Cat Win' automáticamente.
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
