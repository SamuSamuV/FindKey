/// <summary>
/// Class: CommandVocabulary
/// Description: This script defines a ScriptableObject called CommandVocabulary, which is used to store a list of synonyms for commands in the FindKey game. It allows for easy
///              management of command synonyms, enabling the game to recognize different variations of player input as valid commands. The list of synonyms can be edited in the Unity Editor,
///              making it flexible for game designers to add or modify command variations without changing the code.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewVocabulary", menuName = "Adventure/Vocabulary")]
public class CommandVocabulary : ScriptableObject
{
    [Header("Lista de sinónimos")]
    public List<string> synonyms;
}