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
    // Strings for the different locations and interactions in the movement map. These are set in the Unity Inspector and can be easily modified without changing the code.
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
        // We look for the MoveAppData component in the scene to ensure we have a reference to it. This is important for managing the state of the game related to the player's actions in the movement map.
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
            moveAppData = goMoveAppData.GetComponent<MoveAppData>();
    }

    void Start()
    {
        // We look for the MoveAppData component again in Start to ensure we have a reference to it. This is a safety check in case it wasn't found in Awake, although ideally it should be found there.
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
        // If the player doesn't have the axe, we give it to them and update the story log. We also check if the inventory is open to add the axe to it. If they already have the axe, we just update the story log to reflect that.
        if (!moveAppData.hasAxe)
        {
            storyLog.SetTextAnimated(pickAxeText);
            moveAppData.hasAxe = true;

            DesktopManager dm = FindObjectOfType<DesktopManager>(); // We find the DesktopManager in the scene to access its icons and check if the inventory is open.

            // Important, we look for the inventory icon in the desktop manager's icons to spawn, and if it's open, we get its InventoryManager component to add the axe to the inventory. This way we ensure that the inventory is updated correctly based on the player's actions.
            foreach (var data in dm.iconsToSpawn)
            {
                // We look for the icon with the label "Inventario" to find the inventory window.
                if (data.label == "Inventario")
                {
                    // If the inventory window is open, we get its InventoryManager component to add the axe to the inventory. This ensures that the player's inventory is updated in real-time based on their actions in the movement map.
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

        // If the player already has the axe, we just update the story log to reflect that they have already picked it up.
        else
        {
            storyLog.SetTextAnimated(hasAlreadyPickAxeText);
        }
    }

    public void PickUpCorruptedChest()
    {
        // Similar to picking up the axe, if the player doesn't have the corrupted chest, we give it to them and update the story log. We also check if the inventory is open to add the chest to it. If they already have the chest, we just update the story log to reflect that.
        if (!moveAppData.hasChest)
        {
            moveAppData.hasChest = true;

            DesktopManager dm = FindObjectOfType<DesktopManager>(); // We find the DesktopManager in the scene to access its icons and check if the inventory is open.

            // Important, we look for the inventory icon in the desktop manager's icons to spawn, and if it's open, we get its InventoryManager component to add the chest to it. This way we ensure that the inventory is updated correctly based on the player's actions.
            foreach (var data in dm.iconsToSpawn)
            {
                // We look for the icon with the label "Inventario" to find the inventory window.
                if (data.label == "Inventario")
                {
                    // If the inventory window is open, we get its InventoryManager component to add the chest to the inventory. This ensures that the player's inventory is updated in real-time based on their actions in the movement map.
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

        // If the player already has the chest, we just update the story log to reflect that they have already picked it up.
        else
        {
            storyLog.SetTextAnimated(hasAlreadyPickAxeText);
        }
    }

    public void FirstGoStraight()
    {
        storyLog.SetTextAnimated(goFirstStraightText);
    }

    public void ActivateCatUI() // This method is responsible for activating the UI related to the cat encounter. It shows the cat encounter panel and hides the movement panel.
                                // It also disables the close and minimize buttons of the app window to prevent the player from closing it during the encounter.
    {
        IAPanel.SetActive(true);
        MovePanel.SetActive(false);

        AppWindow myApp = GetComponent<AppWindow>();
        if (myApp != null) myApp.SetCloseAndMinimizeInteractable(false);
    }

    public void GoToCatPosition()
    {
        if (moveAppData == null) return;

        if (!moveAppData.catIsDead) // If the cat is not dead, we set the playerIsFrontCat flag to true and check if the enemy encounter app is open. If it is, we update the cat
                                    // encounter data to reflect that the player is in front of the cat and activate the cat UI. If the enemy encounter app is not open, we just
                                    // activate the cat UI without updating any encounter data.
        {
            moveAppData.playerIsFrontCat = true;
            DesktopManager dm = FindObjectOfType<DesktopManager>();
            bool enemyAppOpen = false;

            if (dm != null && dm.iconsToSpawn != null)
            {
                foreach (var data in dm.iconsToSpawn) // We look for the icon with the label "Enemy Encounter" to find the enemy encounter window.
                {
                    if (data.label == "Enemy Encounter") // If we find the enemy encounter we check if its window is open. If it is, we set the playerIsFrontCat
                                                         // flag to true in the moveAppData and update the cat encounter data to reflect that the player is in front of the cat.
                                                         // We also activate the cat UI to show the encounter panel. If the enemy encounter window is not open, we just activate the
                                                         // cat UI without updating any encounter data.
                    {
                        if (data.isOpen && data.windowInstance != null)
                        {
                            enemyAppOpen = true;

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

            if (!enemyAppOpen) // If the enemy encounter app is not open, we just activate the cat UI without updating any encounter data.
            {
                IAPanel.SetActive(false);
                MovePanel.SetActive(true);
                if (playerInputField) playerInputField.SetActive(false);
            }
        }

        else // If the cat is dead, we just set the playerIsFrontCat flag to false and show the movement panel again, allowing the player to continue exploring the
             // movement map without any encounter.
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
