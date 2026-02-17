using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewVocabulary", menuName = "Adventure/Vocabulary")]
public class CommandVocabulary : ScriptableObject
{
    [Header("Lista de sinónimos")]
    public List<string> synonyms;
}