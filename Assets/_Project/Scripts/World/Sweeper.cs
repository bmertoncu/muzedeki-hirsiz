using UnityEngine;

public class Sweeper : MonoBehaviour
{
    // degrees to each side, like a security camera looking left and right
    [SerializeField] private float sweepAngle = 45f;
    [SerializeField] private float sweepSpeed = 1f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation; // remember where we began
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * sweepSpeed) * sweepAngle; // between -45 and +45
        transform.localRotation = startRotation * Quaternion.Euler(0f, offset, 0f);
    }
}
