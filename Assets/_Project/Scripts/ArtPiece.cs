using UnityEngine;

public class ArtPiece : MonoBehaviour
{
    [SerializeField] private int value = 100;
    [SerializeField] private GameObject collectEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ThiefController>(out var thief))
        {
            thief.CollectArt();
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
