using UnityEngine;
// TEST TOOL: not part of the game
public class FrameRateTest : MonoBehaviour
{
[SerializeField] private int targetFps = 10;
void Update()
{
Application.targetFrameRate = targetFps;
}
void OnDisable() // runs when we press Stop
{
Application.targetFrameRate = -1; // no limit
}
}
