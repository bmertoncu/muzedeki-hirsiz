using UnityEngine;

public class Patroller : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2.0f;

    private Vector3 pointA;
    private bool goingToB = true;

    void Start()
    {
        pointA = transform.position;
    }

    void Update()
    {
        if (pointB == null) return;
        Vector3 target = goingToB ? pointB.position : pointA;

        transform.position = Vector3.MoveTowards(transform.position, target, speed*Time.deltaTime);
        if (transform.position == target)
        {
            goingToB = !goingToB;
        }
    }
}