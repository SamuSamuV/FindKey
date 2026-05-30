using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Direction // Enum to represent the possible movement directions in the game, making the code more readable and maintainable when dealing with player movements and position checks.
{
    Left,
    Right,
    Straight
}

public enum Actions // Enum to represent the possible actions the player can take when interacting with objects in the game, such as looking at a painting, picking up an axe,
                    // attacking, or running. This helps to keep track of the player's interactions and can be used for game logic and narrative progression.
{
    Look,
    Pick,
    Atack,
    Run,
}

/// <summary>
/// Class: SelectMove
/// Description: This script manages the player's input for selecting moves in the movement map of the FindKey game. It listens for text input from the player, processes it
///              based on the current position in the movement map, and updates the game's state accordingly. The script checks the player's current location and available actions,
///              then validates the input against expected commands for that position. If the input is valid, it triggers the corresponding move or interaction; if not, it shows an
///              error message. The script also handles adding and removing movements from the player's history and interacts with other components like Moves, StoryLog, and
///              MoveAppManager to manage the narrative and game mechanics based on the player's choices.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class SelectMove : MonoBehaviour
{
    public TMP_InputField inputField;

    public Moves movesScript;
    public StoryLog storyLog;
    public MoveAppManager moveAppManager;
    public MoveAppData moveAppData;

    private void Start() // In the Start method, we initialize the moveAppData reference by finding the GameObject with the "MoveAppData" tag and getting its MoveAppData component.
                         // We also set up a listener for when the player submits text input, which will call the OnTextSubmitted method.
                         // Finally, we call CheckPosition to update the game state based on the initial position.
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        moveAppData = goMoveAppData.GetComponent<MoveAppData>();

        inputField.onSubmit.AddListener(OnTextSubmitted);
        CheckPosition();
    }

    private void OnDestroy() // In the OnDestroy method, we remove the listener for text submission to prevent potential memory leaks or unintended behavior when the object is destroyed.
    {
        inputField.onSubmit.RemoveListener(OnTextSubmitted);
    }

    private bool IsAtFrontPainting() => MatchesSequence(Direction.Left); // This method checks if the player's current position in the movement map matches the sequence of movements thatwould place them in front of the painting, which is represented by a single Left movement.
    private bool IsAtFirstStraightPath() => MatchesSequence(Direction.Straight); // This method checks if the player's current position in the movement map matches the sequence of movements that would place them at the first straight path, which is represented by a single Straight movement.
    private bool IsFrontAxe() => MatchesSequence(Direction.Straight, Direction.Right); // This method checks if the player's current position in the movement map matches the sequence of movements that would place them in front of the axe, which is represented by a Straight movement followed by a Right movement.
    private bool IsFrontCat() => MatchesSequence(Direction.Straight, Direction.Straight); // This method checks if the player's current position in the movement map matches the sequence of movements that would place them in front of the cat, which is represented by two Straight movements.

    private bool IsAtBegining() // This method checks if the player is at the beginning of the movement map, which is determined by having an empty movement history. If there are no movements recorded, it means the player has not moved from the starting position.
    {
        return moveAppManager.movementHistory.Count == 0;
    }

    private void ShowInputError() // This method is responsible for displaying an error message when the player inputs an invalid command. It retrieves the last loaded text from the
                                  // story log and constructs a message that includes the error notification and the previous text for context. The message is then displayed using
                                  // the SetTextAnimated method of the story log.
    {
        string previousText = storyLog.lastLoadedText;

        string message = string.IsNullOrEmpty(previousText)
            ? "<color=red>Acción no válida.</color>"
            : $"<color=red>Acción no válida.</color>\n\n{previousText}";

        storyLog.SetTextAnimated(message);
    }

    private void OnTextSubmitted(string input) // This method is called when the player submits text input. It first checks if the input is null or empty, and if so, it
                                               // simply returns without doing anything. If there is input, it converts it to lowercase and trims any leading or trailing whitespace
                                               // to standardize the input for easier processing.
    {
        if (string.IsNullOrEmpty(input)) return;
        input = input.ToLower().Trim();

        // All this if statements check the player's current position in the movement map and call the corresponding handler method based on where they are.
        // Each handler method will process the input according to the expected commands for that position. If none of the conditions match, it means the player is in an
        // unknown state, and we show an error message.

        if (IsFrontAxe())
        {
            HandleFrontAxeInput(input);
            ResetInput();
            return;
        }

        if (IsFrontCat())
        {
            HandleDeadCatPositionInput(input);
            ResetInput();
            return;
        }

        if (IsAtFrontPainting())
        {
            HandleIsAtFrontPaintingInput(input);
            ResetInput();
            return;
        }

        if (IsAtFirstStraightPath())
        {
            HandleFirstStraightPathInput(input);
            ResetInput();
            return;
        }

        if (IsAtBegining())
        {
            HandleIsAtBeginingInput(input);
            ResetInput();
            return;
        }

        ShowInputError();
        ResetInput();
    }

    private void ResetInput() // This method resets the input field by clearing its text and reactivating it, allowing the player to enter a new command immediately after submitting
                              // their previous input.
    {
        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void HandleIsAtBeginingInput(string input) // This method handles the player's input when they are at the beginning of the movement map. It checks the input against
                                                       // the expected commands for that position (right, left, straight) and calls the AddMovement method with the corresponding
                                                       // direction if the input is valid. If the input does not match any of the expected commands, it shows an error message.
    {
        switch (input)
        {
            case "right":
                AddMovement(Direction.Right);
                break;
            case "left":
                AddMovement(Direction.Left);
                break;
            case "straight":
                AddMovement(Direction.Straight);
                break;
            default:
                ShowInputError();
                break;
        }
    }

    private void HandleIsAtFrontPaintingInput(string input) // This method handles the player's input when they are in front of the painting. It checks the input against the
                                                            // expected commands for that position (look, return) and calls the corresponding method if the input is valid.
                                                            // If the input does not match any of the expected commands, it shows an error message.
    {
        switch (input)
        {
            case "look":
                movesScript.LookPainting();
                moveAppManager.actionsHistory.Add(Actions.Look);
                break;
            case "return":
                RemoveLastMovement();
                storyLog.SetTextAnimated(movesScript.youComeBackToStartText);
                break;
            default:
                ShowInputError();
                break;
        }
    }

    private void HandleFirstStraightPathInput(string input) // This method handles the player's input when they are at the first straight path. It checks the input against the
                                                            // expected commands for that position (right, straight, return)
    {
        switch (input)
        {
            case "right":
                AddMovement(Direction.Right);
                break;
            case "straight":
                AddMovement(Direction.Straight);
                break;
            case "return":
                RemoveLastMovement();
                storyLog.SetTextAnimated(movesScript.youComeBackToStartText);
                break;
            default:
                ShowInputError();
                break;
        }
    }

    private void HandleFrontAxeInput(string input) // This method handles the player's input when they are in front of the axe. It checks the input against the expected commands
                                                   // for that position (pick, return) and calls
    {
        switch (input)
        {
            case "pick":
                movesScript.PickAxe();
                moveAppManager.actionsHistory.Add(Actions.Pick);
                break;
            case "return":
                RemoveLastMovement();
                storyLog.SetTextAnimated(movesScript.goFirstStraightButYouReturnFromTheAxeText);
                break;
            default:
                ShowInputError();
                break;
        }
    }

    private void HandleDeadCatPositionInput(string input) // This method handles the player's input when they are in front of the cat. It checks the input against the expected
                                                          // commands for that position (straight, return) and calls
    {
        switch (input)
        {
            case "straight":
                AddMovement(Direction.Straight);
                break;
            case "return":
                RemoveLastMovement();
                storyLog.SetTextAnimated(movesScript.goFirstStraightButYouReturnFromTheCatPositionText);
                break;
            default:
                ShowInputError();
                break;
        }
    }

    public void AddMovement(Direction direction) // This method adds a movement to the player's movement history. It takes a Direction as a parameter, adds it to the movement history list in the MoveAppManager,
                                                 // and then calls CheckPosition to update the game state based on the new position after the movement is added. It also logs the movement for debugging purposes.
    {
        moveAppManager.movementHistory.Add(direction);
        Debug.Log($"Move {moveAppManager.movementHistory.Count}: {direction}");
        CheckPosition();
    }

    private void RemoveLastMovement() // This method removes the last movement from the player's movement history. It checks if there are any movements in the history, and if so, it removes the last one and logs the removed direction.
                                      // If there are no movements to remove, it shows an error message and logs that there are no movements to remove.
    {
        if (moveAppManager.movementHistory.Count > 0)
        {
            Direction removed = moveAppManager.movementHistory[moveAppManager.movementHistory.Count - 1];
            moveAppManager.movementHistory.RemoveAt(moveAppManager.movementHistory.Count - 1);
            Debug.Log($"Se quitó la última dirección: {removed}");
        }

        else
        {
            ShowInputError();
            Debug.Log("No hay movimientos que quitar");
        }
    }

    private bool MatchesSequence(params Direction[] sequence) // This method checks if the player's movement history exactly matches a given sequence of directions.
    {
        if (moveAppManager.movementHistory.Count != sequence.Length) return false;
        for (int i = 0; i < sequence.Length; i++) // It iterates through the provided sequence of directions and compares each direction with the corresponding direction in the player's movement history. If any direction does not match, it returns false.
            if (moveAppManager.movementHistory[i] != sequence[i]) return false;
        return true;
    }

    private bool MatchesLastMoves(params Direction[] sequence) // This method checks if the last moves in the player's movement history match a given sequence of directions. It first checks if the movement history has enough moves to compare with the sequence.
    {
        if (moveAppManager.movementHistory.Count < sequence.Length) return false;
        int startIndex = moveAppManager.movementHistory.Count - sequence.Length;
        for (int i = 0; i < sequence.Length; i++) // It iterates through the provided sequence of directions and compares each direction with the corresponding direction in the player's movement history, starting from the calculated index. If any direction does not match, it returns false.
            if (moveAppManager.movementHistory[startIndex + i] != sequence[i]) return false;
        return true;
    }

    private void CheckPosition() // This method checks the player's current position in the movement map based on their movement history and updates the game state accordingly.
                                 // It first checks if the player is dead, and if so, it returns without doing anything.
    {
        if (moveAppManager.dead) return;

        if (moveAppManager.movementHistory.Count == 0)
            storyLog.SetTextAnimated(movesScript.startText);

        if (MatchesSequence(Direction.Right)) movesScript.GoFirstRightDie();
        if (MatchesSequence(Direction.Left)) movesScript.GoToPainting();
        if (MatchesSequence(Direction.Straight)) movesScript.FirstGoStraight();
        if (MatchesSequence(Direction.Straight, Direction.Right)) movesScript.GoToAxe();
        if (MatchesSequence(Direction.Straight, Direction.Straight)) movesScript.GoToCatPosition();
        if (MatchesSequence(Direction.Straight, Direction.Straight, Direction.Straight)) movesScript.GoToNextStageAfterCat();
    }
}