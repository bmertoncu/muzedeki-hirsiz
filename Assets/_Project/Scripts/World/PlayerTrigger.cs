using UnityEngine;

// W03: Detect -> Who? -> Act
// Müzedeki her tetikleyici (trigger) aynı şekle sahip:
//   1) Detect : OnTriggerEnter ile içeri girileni yakala
//   2) Who?   : giren şey hırsız mı? (tag ile soruyoruz)
//   3) Act    : mesaj yaz, istersek objeyi sahneden kaldır
// Bu script tek başına kullanılabilir: sadece Message ve Remove After değişiyor.
// Not: Collider'da "Is Trigger" açık olmalı, hırsızda da Collider + Rigidbody/CharacterController olmalı.
public class PlayerTrigger : MonoBehaviour
{
    // Inspector'dan her obje için ayrı ayrı ayarlanır
    [SerializeField] private string message = "Toplandı!";
    [SerializeField] private bool removeAfter = true;
    [SerializeField] private string requiredTag = "Player";

    [Header("Editörde göster (trigger görünmez, o yüzden çizgi çiziyoruz)")]
    [SerializeField] private Color gizmoColor = new Color(0.2f, 0.9f, 0.4f, 1f);

    void OnTriggerEnter(Collider other)
    {
        // Who? Sadece "Player" tag'li obje (hırsız) geçerli
        if (!other.CompareTag(requiredTag)) return;

        // Act: haber ver, istenirse objeyi kaldır
        Debug.Log(message);
        if (removeAfter) Destroy(gameObject);
    }

    // Affordance var ama signifier yok: trigger görünmez.
    // Sahne ekranında sınırlarını çizerek tasarımcıya (bana) ipucu veriyoruz.
    void OnDrawGizmos()
    {
        Collider c = GetComponent<Collider>();
        if (c == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
    }
}
