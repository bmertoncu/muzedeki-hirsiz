using UnityEngine;

public class ArtPiece : MonoBehaviour
{
    private Rigidbody rb;
    private Collider solidCollider;
    private Collider pickupTrigger;
    private float canPickUpAt;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        solidCollider = GetComponent<BoxCollider>();
        pickupTrigger = GetComponent<SphereCollider>();
    }

    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        if (rb != null) rb.isKinematic = true;
        if (solidCollider != null) solidCollider.enabled = false;
        if (pickupTrigger != null) pickupTrigger.enabled = false;

        if (TryGetComponent<Bobber>(out var bobber)) bobber.enabled = false;

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null);
        if (solidCollider != null) solidCollider.enabled = true;
        if (pickupTrigger != null) pickupTrigger.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = false;
            Vector3 push = -transform.forward + Vector3.up * 1.5f;
            rb.AddForce(push * 4f, ForceMode.Impulse);
        }

        if (TryGetComponent<Bobber>(out var bobber)) bobber.enabled = true;

        canPickUpAt = Time.time + 0.6f;
    }
}
