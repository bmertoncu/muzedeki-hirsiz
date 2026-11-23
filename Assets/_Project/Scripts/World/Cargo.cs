using UnityEngine;

public class Cargo : MonoBehaviour
{
    // light or heavy? a data file decides
    [SerializeField] private CargoType type;

    private Rigidbody rb;
    private Collider box;      // solid body
    private Collider range;    // pickup trigger
    private float canPickUpAt; // re-pickup time

    void Awake()
    {
        // find our own components once, keep them
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();

        // an empty Type slot? say it in red
        if (type == null) Debug.LogError(name + ": no Cargo Type");
    }

    // two questions about this cargo, answered by its type
    public int Points()
    {
        return type.Points();
    }

    public float Weight()
    {
        return type.Weight();
    }

    // a question: "can I be picked up now?"
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true;      // physics stops
        box.enabled = false;        // no bumping
        range.enabled = false;      // no re-pickup

        // become a child of the carry point
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null);  // free again
        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;     // physics again

        // push it back and up, away from the courier
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * 3f, ForceMode.Impulse);

        // wait 0.5 s before it can be picked up again
        canPickUpAt = Time.time + 0.5f;
    }
}
