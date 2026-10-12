using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        Cargo cargo = carry.HandOver();
        if (cargo != null)
        {
            Destroy(cargo.gameObject);
            Debug.Log("Delivered!");
        }
    }
}
