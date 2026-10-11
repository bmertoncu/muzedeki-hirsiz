using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private int requiredArtPieces = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ThiefController>(out var thief))
        {
            if (thief.collectedArtCount >= requiredArtPieces)
            {
                thief.EscapeSuccess();
            }
            else
            {
                Debug.Log($"[Müzedeki Hırsız] Çıkış kilitli! Daha fazla sanat eseri toplamalısın ({thief.collectedArtCount}/{requiredArtPieces}).");
            }
        }
    }
}
