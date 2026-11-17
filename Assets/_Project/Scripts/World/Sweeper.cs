using UnityEngine;

public class Sweeper : MonoBehaviour
{
    // Kameranın iki yöne döneceği açı
    [SerializeField] private float sweepAngle = 45f;
    [SerializeField] private float sweepSpeed = 1f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation.normalized;

        // Kameranın dönüşünü yavaşlatır
        sweepSpeed = 0.3f;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * sweepSpeed) * sweepAngle;

        transform.localRotation =
            (startRotation * Quaternion.Euler(0f, offset, 0f)).normalized;
    }
}