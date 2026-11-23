using UnityEngine;

public class CameraZone : MonoBehaviour
{
    // the Cinemachine Camera to switch on in this zone
    [SerializeField] private GameObject zoneCamera;

    // Detect: something enters the zone
    void OnTriggerEnter(Collider other)
    {
        // Who? only the object tagged "Player"
        if (!other.CompareTag("Player")) return;

        // Act: switch the zone camera on: it becomes live
        zoneCamera.SetActive(true);
    }

    // Detect: something leaves the zone
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // switch it off: the follow camera is live again
        zoneCamera.SetActive(false);
    }
}
