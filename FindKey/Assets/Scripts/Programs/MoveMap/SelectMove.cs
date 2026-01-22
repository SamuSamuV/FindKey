using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Straight
}

public enum Actions
{
    Look,
    Pick,
    Atack,
    Run,

}

public class SelectMove : MonoBehaviour
{
    public TMP_InputField inputField;

    public Moves movesScript;
    public StoryLog storyLog;
    public MoveAppManager moveAppManager;
    public MoveAppData moveAppData;

    private void Start()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        moveAppData = goMoveAppData.GetComponent<MoveAppData>();

        inputField.onSubmit.AddListener(OnTextSubmitted);
        CheckPosition();
    }

    private void OnDestroy()
    {
        inputField.onSubmit.RemoveListener(OnTextSubmitted);
    }

    private bool IsAtFrontPainting()
    {
        return MatchesSequence(Direction.Left);
    }

    private bool IsAtFirstStraightPath()
    {
        return MatchesSequence(Direction.Straight);
    }

    private bool IsFrontAxe()
    {
        return MatchesSequence(Direction.Straight, Direction.Right);
    }

    private bool IsFrontCat()
    {
        return MatchesSequence(Direction.Straight, Direction.Straight);
    }

    private bool IsAtBegining()
    {
        if (moveAppManager.movementHistory.Count == 0)
            return true;

        else
            return false;
    }

    private void OnTextSubmitted(string input)
    {
        input = input.ToLower().Trim();

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
    }

    private void ResetInput()
    {
        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void HandleIsAtBeginingInput(string input)
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
                storyLog.SetText("You can't do that here.");
                break;
        }
    }

    private void HandleIsAtFrontPaintingInput(string input)
    {
        switch (input)
        {
            case "look":
                movesScript.LookPainting();
                moveAppManager.actionsHistory.Add(Actions.Look);
                break;

            case "return":
                RemoveLastMovement();
                storyLog.SetText(movesScript.youComeBackToStartText);
                break;

            default:
                storyLog.SetText("You can't do that here.");
                break;
        }
    }

    private void HandleFirstStraightPathInput(string input)
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
                storyLog.SetText(movesScript.youComeBackToStartText);
                break;

            default:
                storyLog.SetText("You can't do that here.");
                break;
        }
    }

    private void HandleFrontAxeInput(string input)
    {
        switch (input)
        {
            case "pick":
                movesScript.PickAxe();
                moveAppManager.actionsHistory.Add(Actions.Pick);
                break;

            case "return":
                RemoveLastMovement();
                storyLog.SetText(movesScript.goFirstStraightButYouReturnFromTheAxeText);
                break;

            default:
                storyLog.SetText("You can't do that here.");
                break;
        }
    }

    private void HandleDeadCatPositionInput(string input)
    {
        switch (input)
        {
            case "straight":
                AddMovement(Direction.Straight);
                break;

            case "return":
                RemoveLastMovement();
                storyLog.SetText(movesScript.goFirstStraightButYouReturnFromTheCatPositionText);
                break;

            default:
                storyLog.SetText("You can't do that here.");
                break;
        }
    }

    public void AddMovement(Direction direction)
    {
        moveAppManager.movementHistory.Add(direction);

        Debug.Log($"Move {moveAppManager.movementHistory.Count}: {direction}");
        CheckPosition();
    }

    private void RemoveLastMovement()
    {
        if (moveAppManager.movementHistory.Count > 0)
        {
            Direction removed = moveAppManager.movementHistory[moveAppManager.movementHistory.Count - 1];
            moveAppManager.movementHistory.RemoveAt(moveAppManager.movementHistory.Count - 1);
            Debug.Log($"Se quitó la última dirección: {removed}");
        }

        else
        {
            storyLog.SetText("You can't go back.");
            Debug.Log("No hay movimientos que quitar");
        }
    }

    private bool MatchesSequence(params Direction[] sequence)
    {
        if (moveAppManager.movementHistory.Count != sequence.Length)
            return false;

        for (int i = 0; i < sequence.Length; i++)
        {
            if (moveAppManager.movementHistory[i] != sequence[i])
                return false;
        }

        return true;
    }

    private bool MatchesLastMoves(params Direction[] sequence)
    {
        if (moveAppManager.movementHistory.Count < sequence.Length)
            return false;

        int startIndex = moveAppManager.movementHistory.Count - sequence.Length;

        for (int i = 0; i < sequence.Length; i++)
        {
            if (moveAppManager.movementHistory[startIndex + i] != sequence[i])
                return false;
        }

        return true;
    }

    private void CheckPosition()
    {
        if (moveAppManager.dead)
            return;

        if (moveAppManager.movementHistory.Count == 0)
        {
            storyLog.SetText(movesScript.startText);
        }

        if (MatchesSequence(Direction.Right))
        {
            movesScript.GoFirstRightDie();
        }

        if (MatchesSequence(Direction.Left))
        {
            movesScript.GoToPainting();
        }

        if (MatchesSequence(Direction.Straight))
        {
            movesScript.FirstGoStraight();
        }

        if (MatchesSequence(Direction.Straight, Direction.Right))
        {
            movesScript.GoToAxe();
        }

        if (MatchesSequence(Direction.Straight, Direction.Straight))
        {
            movesScript.GoToCatPosition();
        }

        if (MatchesSequence(Direction.Straight, Direction.Straight, Direction.Straight))
        {
            movesScript.GoToNextStageAfterCat();
        }
    }
}