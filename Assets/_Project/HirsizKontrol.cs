using UnityEngine;

public class HirsizKontrol : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Hareket ve Zýplama Ayarlarý")]
    public float yurumeHizi = 5.0f;
    public float gravity = -9.81f;
    public float normalZiplamaGucu = 2.0f;
    public float azZiplamaGucu = 0.1f;

    [Header("Aðýrlýk Sistemi")]
    public int karakterGecisAgirligi = 1;
    public int toplamAgirlik;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        toplamAgirlik = karakterGecisAgirligi;
    }

    void Update()
    {
        // Zemin Kontrolü
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Yürüme Kontrolü (WASD - Gerçek Dünya Eksenine Göre)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z);
        controller.Move(move * yurumeHizi * Time.deltaTime);

        // Karakterin Yüzünü Yürüdüðü Yöne Dönmesi
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        // Zýplama Girdisi (Space)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Zipla();
        }

        // Yerçekimi Uygulamasý
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Zipla()
    {
        float uygulanacakZiplamaGucu = (toplamAgirlik >= 4) ? azZiplamaGucu : normalZiplamaGucu;
        velocity.y = Mathf.Sqrt(uygulanacakZiplamaGucu * -2f * gravity);
    }

    public void EsyaAldi(GameObject alinanEsya)
    {
        EsyaOzellikleri esyaOzellik = alinanEsya.GetComponent<EsyaOzellikleri>();
        if (esyaOzellik != null)
        {
            toplamAgirlik = karakterGecisAgirligi + esyaOzellik.agirlik;
        }
    }

    public void EsyaBirakti()
    {
        toplamAgirlik = karakterGecisAgirligi;
    }
}