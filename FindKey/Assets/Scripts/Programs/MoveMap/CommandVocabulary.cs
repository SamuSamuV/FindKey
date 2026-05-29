/// <summary>
/// Class: CommandVocabulary
/// Descripción: ScriptableObject que representa el vocabulario de comandos disponibles en el juego.
/// Autor: Samuel Campos Borrego
/// Proyecto: FindKey
/// </summary>

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewVocabulary", menuName = "Adventure/Vocabulary")]
public class CommandVocabulary : ScriptableObject
{
    [Header("Lista de sinónimos")]
    public List<string> synonyms;
}