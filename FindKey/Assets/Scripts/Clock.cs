using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class Clock : MonoBehaviour
{
    public TextMeshProUGUI timeText; // formato: HH:mm
    public TextMeshProUGUI dateText; // formato: d MMM yyyy


    private void Start()
    {
        if (timeText == null || dateText == null) enabled = false;
    }


    private void Update()
    {
        DateTime now = DateTime.Now;
        timeText.text = now.ToString("HH:mm");
        dateText.text = now.ToString("dddd, d MMM yyyy");
    }
}