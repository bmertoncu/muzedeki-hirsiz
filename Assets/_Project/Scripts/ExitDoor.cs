using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [Header("Bölüm Geçişi")]
    [Tooltip("Bu bölüm bittiğinde yüklenecek sahnenin adı. Boş bırakılırsa yeni sahne yüklenmez.")]
    [SerializeField] private string nextSceneName = "bolum-2";

    [Tooltip("Bölüm tamamlandıktan sonra yeni sahneye geçmeden önceki bekleme süresi (saniye).")]
    [SerializeField] private float delayBeforeNextScene = 1.5f;

    private bool levelFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (levelFinished) return;

        // 1. ThiefController kontrolü
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
                    levelFinished = true;
                    Debug.Log("<color=green>[MÜZEDEKİ HIRSIZ] TEBRİKLER! TÜM SANAT ESERLERİNİ ÇALIP BÖLÜMÜ TAMAMLADINIZ!</color>");
                    TriggerNextLevel();
                }
                else
                {
                    Debug.Log($"[Müze Çıkışı] Harika! Kalan eserler: {thief.totalArtInLevel - thief.totalDeliveredArt}");
                }
            }
        }
        // 2. PlayerMove kontrolü (yedek/uyumluluk desteği)
        else if (other.TryGetComponent<PlayerMove>(out var pm))
        {
            if (pm.IsCarrying())
            {
                ArtPiece art = pm.HandOverArt();
                if (art != null)
                {
                    Destroy(art.gameObject);
                }

                if (pm.totalDeliveredArt >= pm.totalArtInLevel)
                {
                    pm.hasEscaped = true;
                    levelFinished = true;
                    Debug.Log("<color=green>[MÜZEDEKİ HIRSIZ] TEBRİKLER! TÜM SANAT ESERLERİNİ ÇALIP BÖLÜMÜ TAMAMLADINIZ!</color>");
                    TriggerNextLevel();
                }
                else
                {
                    Debug.Log($"[Müze Çıkışı] Harika! Kalan eserler: {pm.totalArtInLevel - pm.totalDeliveredArt}");
                }
            }
        }
    }

    private void TriggerNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"<color=cyan>[Müze Çıkışı] {delayBeforeNextScene} saniye içinde {nextSceneName} sahnesine geçiliyor...</color>");
            Invoke(nameof(LoadNextLevel), delayBeforeNextScene);
        }
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
