using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;

    private Cargo carried;     // null = hands empty
    private Rigidbody rb;      // the courier's body
    private float baseMass;    // its mass with empty hands

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;
    }

    public bool IsCarrying()
    {
        return carried != null;
    }

    // runs when the courier enters a trigger
    void OnTriggerEnter(Collider other)
    {
        Cargo cargo = other.GetComponent<Cargo>();
        if (cargo == null) return;                // not cargo
        if (IsCarrying()) return;                 // hands full
        if (!cargo.CanBePickedUp()) return;       // just dropped

        carried = cargo;
        cargo.PickUp(carryPoint);
        rb.mass = baseMass + cargo.Weight();      // heavier courier
    }

    // the hazards and the E key call this
    public void DropCargo()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
        rb.mass = baseMass;                       // light again
    }

    // the green zone calls this
    public Cargo HandOver()
    {
        Cargo cargo = carried;
        carried = null;                           // hands empty again
        rb.mass = baseMass;                       // light again
        return cargo;
    }
}
