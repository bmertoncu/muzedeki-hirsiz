using UnityEngine;

// W03 shape: Detect -> Who? -> Act, with the W04 grip idea from Slippery
public class WetFloor : MonoBehaviour
{
    // 1 = normal ground, small = slippery
    [SerializeField] private float grip = 0.3f;

    // Detect: someone steps on the wet floor
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ThiefController>(out var thief))
        {
            thief.SetGrip(grip);
        }
        else if (other.TryGetComponent<PlayerMove>(out var pm))
        {
            pm.SetGrip(grip);
        }
    }

    // Detect: the thief leaves the wet floor
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ThiefController>(out var thief))
        {
            thief.SetGrip(1f);   // full grip again
        }
        else if (other.TryGetComponent<PlayerMove>(out var pm))
        {
            pm.SetGrip(1f);
        }
    }
}
