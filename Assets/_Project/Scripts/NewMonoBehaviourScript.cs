using UnityEngine;
public class Spinner : MonoBehaviour
{
<<<<<<< HEAD
void Update()
{
transform.Rotate(0f, 1f, 0f); // 1 degree, every frame
=======
// degrees per SECOND
[SerializeField] private float rotationSpeed = 90f;
void Update()
{
transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
}
>>>>>>> fee25a2b980978c60a500888c3769fe6fd30a698
}
}