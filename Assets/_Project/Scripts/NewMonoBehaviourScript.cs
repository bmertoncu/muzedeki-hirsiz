using UnityEngine;
public class Spinner : MonoBehaviour
{
void Update()
{
transform.Rotate(0f, 1f, 0f); // 1 degree, every frame
}
}