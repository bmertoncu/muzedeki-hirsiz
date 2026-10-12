using UnityEngine;

public class SecurityZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ThiefController>(out var thief))
        {
            thief.SpotByCamera();
        }
    }
}
