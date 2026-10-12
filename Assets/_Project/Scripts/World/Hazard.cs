using UnityEngine;

public class Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        carry.DropCargo();
        Debug.Log(name + " knocked the cargo off!");
    }
}
