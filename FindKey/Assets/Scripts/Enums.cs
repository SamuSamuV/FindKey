using UnityEngine;
/// <summary>
/// Class: Enums
/// Description: This class contains the enumerations used in the FindKey project. It defines the possible directions and actions that can be performed by the characters in the game.
///              The Direction enum includes Left, Right, and Straight, while the Actions enum includes Look, Pick, Atack, and Run. These enums help to organize and manage the different
///              states and behaviors of the characters in a clear and efficient way.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

public class Enums : MonoBehaviour
{
    public enum Direction // Enumeration for possible directions in the game
    {
        Left,
        Right,
        Straight
    }

    public enum Actions // Enumeration for possible actions that player can perform in the game
    {
        Look,
        Pick,
        Atack,
        Run,
    }
}
