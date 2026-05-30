using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

/// <summary>
/// Class: Clock
/// Description: This script manages the display of the current time and date in the FindKey game. It references two TextMeshProUGUI components,
///              one for displaying the time in "HH:mm" format and another for displaying the date in "dddd, d MMM yyyy" format. The script updates these text elements every frame to ensure that
///              they always show the current time and date. If either of the text references is not assigned, the script will disable itself to prevent errors.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class Clock : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;


    private void Start()
    {
        if (timeText == null || dateText == null) enabled = false;
    }


    private void Update()
    {
        DateTime now = DateTime.Now;
        timeText.text = now.ToString("HH:mm"); // Format of time: hours and minutes in 24-hour format
        dateText.text = now.ToString("dddd, d MMM yyyy"); // Format of date: full day name, day of the month, abbreviated month name, and full year
    }
}