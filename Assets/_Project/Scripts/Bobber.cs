using UnityEngine;
public class Bobber : MonoBehaviour
{
[SerializeField] private float bobHeight = 0.25f; // metres up and down
[SerializeField] private float bobSpeed = 2f; // how fast it floats
private Vector3 startLocalPosition;
void Start()
{
startLocalPosition = transform.localPosition; // remember where we began
}
void Update()
{
float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight; // between -0.25 and +0.25
transform.localPosition = startLocalPosition + Vector3.up * offset; // Vector3.up 0,1,0 , if offset is negative

}
}
