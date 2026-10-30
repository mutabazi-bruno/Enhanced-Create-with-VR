using System.Collections;
using UnityEngine;

// The Quest mutes an app while the headset is off your head. If the app started like that
// (for example Build and Run with the headset on the desk), its sound stayed muted even after
// putting the headset on. So every time the app gets focus back, this stops Unity's sound
// output and starts it again, which opens a new, unmuted sound stream.
public class RestartAudioOnFocus : MonoBehaviour
{
    [Tooltip("Seconds to wait after getting focus, so the Quest has unmuted the app first")]
    public float delay = 1f;

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) return;

        StopAllCoroutines();
        StartCoroutine(RestartAudio());
    }

    IEnumerator RestartAudio()
    {
        yield return new WaitForSecondsRealtime(delay);

        AudioSettings.Mobile.StopAudioOutput();
        yield return null;
        AudioSettings.Mobile.StartAudioOutput();
    }
}
