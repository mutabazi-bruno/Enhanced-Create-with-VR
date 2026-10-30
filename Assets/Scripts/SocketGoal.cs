using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Watches a group of sockets and runs OnAllFilled once every one of them is holding its object.
public class SocketGoal : MonoBehaviour
{
    public XRSocketInteractor[] sockets;

    public UnityEvent OnAllFilled = new UnityEvent();

    private bool done = false;

    void Update()
    {
        if (done) return;

        foreach (XRSocketInteractor socket in sockets)
        {
            // Still waiting for this one
            if (!socket.hasSelection) return;
        }

        done = true;
        OnAllFilled.Invoke();
    }
}
