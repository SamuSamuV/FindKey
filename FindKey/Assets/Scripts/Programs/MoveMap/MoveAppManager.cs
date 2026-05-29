/// <summary>
/// Class: MoveAppManager
/// Description: This script manages the overall state of the movement map application in the FindKey game. It keeps track of whether the player is dead and maintains a
///              history of the player's movements and actions. The movement history is stored as a list of directions (left, right, straight), while the actions history is stored as a
///              list of actions (look, pick, attack, run). This data can be used by other scripts to determine the player's current position in the movement map, what actions they have taken,
///              and to manage the game's narrative and mechanics based on the player's choices and progress in the movement map.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using System.Collections.Generic;
using UnityEngine;

public class MoveAppManager : MonoBehaviour
{
    public bool dead;
    public List<Direction> movementHistory = new List<Direction>();
    public List<Actions> actionsHistory = new List<Actions>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
