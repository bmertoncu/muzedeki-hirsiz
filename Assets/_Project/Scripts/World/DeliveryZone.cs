using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    // the Round keeps the score now
    [SerializeField] private Round round;

    void OnTriggerEnter(Collider other)
    {
        // Who? the courier, carrying cargo
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Act: take the cargo, pay its points, remove it
        Cargo cargo = carry.HandOver();
        round.AddPoints(cargo.Points());
        Destroy(cargo.gameObject);
    }
}
