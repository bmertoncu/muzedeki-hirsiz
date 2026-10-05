using UnityEngine;
public class Spinner : MonoBehaviour
{
[SerializeField] private float rotationSpeed = 1f;
void Update()
{
transform.Rotate(0f, rotationSpeed, 0f);
}
}
