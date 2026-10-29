using UnityEngine;
using UnityEngine.InputSystem;

// Bends the fingers of a hand model with the controller buttons:
// the trigger curls the index finger, the grip curls the other fingers and the thumb.
public class HandAnimator : MonoBehaviour
{
    public InputActionProperty triggerAction;
    public InputActionProperty gripAction;

    [Tooltip("How far each finger joint bends when the button is fully pressed (degrees)")]
    public float fingerBend = 75f;
    public float thumbBend = 35f;

    [Tooltip("How fast the fingers follow the buttons")]
    public float speed = 10f;

    private Transform[] indexJoints;
    private Transform[] otherJoints;
    private Transform[] thumbJoints;

    private Quaternion[] indexStart;
    private Quaternion[] otherStart;
    private Quaternion[] thumbStart;

    private float trigger;
    private float grip;

    void Awake()
    {
        indexJoints = FindJoints("IndexProximal", "IndexIntermediate", "IndexDistal");
        otherJoints = FindJoints("MiddleProximal", "MiddleIntermediate", "MiddleDistal",
                                 "RingProximal", "RingIntermediate", "RingDistal",
                                 "LittleProximal", "LittleIntermediate", "LittleDistal");
        thumbJoints = FindJoints("ThumbProximal", "ThumbDistal");

        // Remember the open hand pose
        indexStart = StartRotations(indexJoints);
        otherStart = StartRotations(otherJoints);
        thumbStart = StartRotations(thumbJoints);
    }

    void Update()
    {
        // Move towards the button values a little each frame so the fingers move smoothly
        trigger = Mathf.MoveTowards(trigger, ReadValue(triggerAction), speed * Time.deltaTime);
        grip = Mathf.MoveTowards(grip, ReadValue(gripAction), speed * Time.deltaTime);

        Bend(indexJoints, indexStart, trigger * fingerBend);
        Bend(otherJoints, otherStart, grip * fingerBend);
        Bend(thumbJoints, thumbStart, grip * thumbBend);
    }

    float ReadValue(InputActionProperty property)
    {
        if (property.action == null) return 0f;
        return property.action.ReadValue<float>();
    }

    void Bend(Transform[] joints, Quaternion[] start, float angle)
    {
        // Every joint bends around its own X axis, towards the palm
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].localRotation = start[i] * Quaternion.Euler(angle, 0f, 0f);
        }
    }

    Transform[] FindJoints(params string[] names)
    {
        Transform[] joints = new Transform[names.Length];
        Transform[] children = GetComponentsInChildren<Transform>();

        for (int i = 0; i < names.Length; i++)
        {
            foreach (Transform child in children)
            {
                // The joints are called L_IndexProximal, R_IndexProximal and so on
                if (child.name.EndsWith(names[i])) joints[i] = child;
            }
        }
        return joints;
    }

    Quaternion[] StartRotations(Transform[] joints)
    {
        Quaternion[] rotations = new Quaternion[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            rotations[i] = joints[i].localRotation;
        }
        return rotations;
    }
}
