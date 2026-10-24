using UnityEngine;

// W03 shape: Detect -> Who? -> Act, with the W04 grip idea from Slippery
public class WetFloor : MonoBehaviour
{
    // 1 = normal ground, small = slippery
    [SerializeField] private float grip = 0.3f;

    // Detect: someone steps on the wet floor
    void OnTriggerEnter(Collider other)
    {
        // Who? only the thief has ThiefController
        ThiefController thief = other.GetComponent<ThiefController>();

        // Act: the thief loses its grip
        if (thief != null) thief.SetGrip(grip);
    }

    // Detect: the thief leaves the wet floor
    void OnTriggerExit(Collider other)
    {
        ThiefController thief = other.GetComponent<ThiefController>();
        if (thief != null) thief.SetGrip(1f);   // full grip again
    }
}
