using System;
using TMPro;
using UnityEngine;

// Shows the current local time (DateTime.Now) as text, like a digital wall clock.
public class DigitalClock : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text dateText;
    public bool use24Hour = true;

    private int lastSecond = -1;

    void Update()
    {
        DateTime now = DateTime.Now;

        // The text only needs to change once a second
        if (now.Second == lastSecond) return;
        lastSecond = now.Second;

        if (timeText != null) timeText.text = now.ToString(use24Hour ? "HH:mm:ss" : "hh:mm:ss tt");
        if (dateText != null) dateText.text = now.ToString("dddd d MMMM");
    }
}
