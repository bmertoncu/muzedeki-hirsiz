using UnityEngine;

public class EsyaEtkilesimi : MonoBehaviour
{
    [Header("Taþýma Ayarlarý")]
    public Transform tasimaNoktasi;
    public float etkilesimMesafesi = 2f;
    public LayerMask esyaKatmani;

    private GameObject tasinanEsya = null;
    private HirsizKontrol hirsizKontrol; // Zýplama koduna eriþim

    void Start()
    {
        hirsizKontrol = GetComponent<HirsizKontrol>();
    }

    void Update()
    {
        // E tuþuna basýldýðýnda alma veya býrakma iþlemi yap
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (tasinanEsya == null) EsyaAl();
            else EsyaBirak();
        }
    }

    void EsyaAl()
    {
        Collider[] colliderlar = Physics.OverlapSphere(transform.position, etkilesimMesafesi, esyaKatmani);
        if (colliderlar.Length > 0)
        {
            tasinanEsya = colliderlar[0].gameObject;

            Rigidbody rb = tasinanEsya.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            tasinanEsya.transform.position = tasimaNoktasi.position;
            tasinanEsya.transform.rotation = tasimaNoktasi.rotation;
            tasinanEsya.transform.SetParent(tasimaNoktasi);

            // Aðýrlýk sistemine haber ver
            if (hirsizKontrol != null) hirsizKontrol.EsyaAldi(tasinanEsya);
        }
    }

    void EsyaBirak()
    {
        tasinanEsya.transform.SetParent(null);

        Rigidbody rb = tasinanEsya.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // Aðýrlýk sistemine haber ver
        if (hirsizKontrol != null) hirsizKontrol.EsyaBirakti();

        tasinanEsya = null;
    }

    // Unity Editör'de etkileþim alanýný sarý bir küre olarak görmek için
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, etkilesimMesafesi);
    }
}