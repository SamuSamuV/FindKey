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

    // --- MÉTODOS DE COMPROBACIÓN ---
    private bool IsAtFrontPainting() => MatchesSequence(Direction.Left);
    private bool IsAtFirstStraightPath() => MatchesSequence(Direction.Straight);
    private bool IsFrontAxe() => MatchesSequence(Direction.Straight, Direction.Right);
    private bool IsFrontCat() => MatchesSequence(Direction.Straight, Direction.Straight);

    private bool IsAtBegining()
    {
        return moveAppManager.movementHistory.Count == 0;
    }

    private void ShowInputError()
    {
        // 1. Recuperamos el último texto válido que mostró el StoryLog
        string previousText = storyLog.lastLoadedText;

        // 2. Creamos el mensaje: Error en Rojo + Salto de línea + Texto original
        // Si no había texto previo, solo mostramos el error.
        string message = string.IsNullOrEmpty(previousText)
            ? "<color=red>You can't do that here.</color>"
            : $"<color=red>You can't do that here.</color>\n\n{previousText}";

        // 3. Lo mostramos animado
        storyLog.SetTextAnimated(message);
    }

    private void OnTextSubmitted(string input)
    {
        if (string.IsNullOrEmpty(input)) return;
        input = input.ToLower().Trim();

        if (IsFrontAxe())
        {
            HandleFrontAxeInput(input);
            ResetInput();
            return;
        }

        if (IsFrontCat())
        {
            HandleDeadCatPositionInput(input); // Solo entra aquí si el gato está muerto (o falló el if de arriba)
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

        // Si estamos en un limbo desconocido
        ShowInputError();
        ResetInput();
    }

    private void ResetInput()
    {
        inputField.text = "";
        inputField.ActivateInputField();
    }

    // --- HANDLERS MODIFICADOS PARA USAR ShowInputError() ---

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
                ShowInputError(); // <--- CAMBIO AQUÍ
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
                storyLog.SetTextAnimated(movesScript.youComeBackToStartText);
                break;
            default:
                ShowInputError(); // <--- CAMBIO AQUÍ
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
                storyLog.SetTextAnimated(movesScript.youComeBackToStartText);
                break;
            default:
                ShowInputError(); // <--- CAMBIO AQUÍ
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
                storyLog.SetTextAnimated(movesScript.goFirstStraightButYouReturnFromTheAxeText);
                break;
            default:
                ShowInputError(); // <--- CAMBIO AQUÍ
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
                storyLog.SetTextAnimated(movesScript.goFirstStraightButYouReturnFromTheCatPositionText);
                break;
            default:
                ShowInputError(); // <--- CAMBIO AQUÍ
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
            ShowInputError();
            Debug.Log("No hay movimientos que quitar");
        }
    }

    // ... (Tus métodos MatchesSequence y MatchesLastMoves siguen igual) ...
    private bool MatchesSequence(params Direction[] sequence)
    {
        if (moveAppManager.movementHistory.Count != sequence.Length) return false;
        for (int i = 0; i < sequence.Length; i++)
            if (moveAppManager.movementHistory[i] != sequence[i]) return false;
        return true;
    }

    private bool MatchesLastMoves(params Direction[] sequence)
    {
        if (moveAppManager.movementHistory.Count < sequence.Length) return false;
        int startIndex = moveAppManager.movementHistory.Count - sequence.Length;
        for (int i = 0; i < sequence.Length; i++)
            if (moveAppManager.movementHistory[startIndex + i] != sequence[i]) return false;
        return true;
    }

    private void CheckPosition()
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