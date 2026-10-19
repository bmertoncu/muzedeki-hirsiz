using UnityEngine;

public class SecurityZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Detect(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Detect(other);
    }

    private void Detect(Collider other)
    {
        if (other.TryGetComponent<ThiefController>(out var thief))
        {
            thief.SpotByCamera();
        }
    }
}
