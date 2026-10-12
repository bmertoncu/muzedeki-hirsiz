using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ThiefController>(out var thief))
        {
            if (thief.IsCarrying())
            {
                ArtPiece art = thief.HandOverArt();
                if (art != null)
                {
                    Destroy(art.gameObject);
                }

                if (thief.totalDeliveredArt >= thief.totalArtInLevel)
                {
                    thief.hasEscaped = true;
                    Debug.Log("<color=green>[MÜZEDEKİ HIRSIZ] TEBRİKLER! TÜM SANAT ESERLERİNİ ÇALDIP BÖLÜMÜ TAMAMLADINIZ!</color>");
                }
                else
                {
                    Debug.Log($"[Müze Çıkışı] Harika! Kalan eserler: {thief.totalArtInLevel - thief.totalDeliveredArt}");
                }
            }
        }
    }
}
