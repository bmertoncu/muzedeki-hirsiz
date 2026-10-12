using UnityEngine;

public class Cargo : MonoBehaviour
{
    private Rigidbody rb;
    private Collider box; // solid body
    private Collider range; // pickup trigger
    private float canPickUpAt; // re-pickup time

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
    }

    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        if (rb != null) rb.isKinematic = true;
        if (box != null) box.enabled = false;
        if (range != null) range.enabled = false;

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null);
        if (box != null) box.enabled = true;
        if (range != null) range.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = false;
            Vector3 push = -transform.forward + Vector3.up;
            rb.AddForce(push * 3f, ForceMode.Impulse);
        }
        canPickUpAt = Time.time + 0.5f;
    }
}
