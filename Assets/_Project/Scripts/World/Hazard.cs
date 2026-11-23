using UnityEngine;
using Unity.Cinemachine;

public class Hazard : MonoBehaviour
{
    private CinemachineImpulseSource shake;   // null = no shake

    void Start()
    {
        // the Impulse Source on this same hazard, if it has one
        shake = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Who? the courier, carrying cargo
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Act: knock the cargo off
        carry.DropCargo();
        Debug.Log(name + " knocked the cargo off!");

        // ...and give the camera a short bump
        if (shake != null) shake.GenerateImpulse();
    }
}
