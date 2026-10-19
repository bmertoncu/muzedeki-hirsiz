using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Hareket ve Zýplama")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 1.1f;

    [Header("Kutu Taþýma (Carrying)")]
    [SerializeField] private Transform carryPoint;
    public ArtPiece carriedArt;

    [Header("Oyun Ýlerlemesi")]
    public int totalDeliveredArt = 0;
    public int totalArtInLevel = 5;
    public bool isCaught = false;
    public bool hasEscaped = false;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;
    private float grip = 1f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");

        // Kutu taþýma noktasýný otomatik bul (eðer atanmamýþsa)
        if (carryPoint == null)
        {
            Transform found = transform.Find("CarryPoint");
            if (found != null) carryPoint = found;
        }
    }

    void Update()
    {
        // Yakalandýysa veya kaçtýysa hiçbir þey yapma
        if (isCaught || hasEscaped) return;

        // Klavyeden E tuþuna basýlýrsa eseri/kutuyu býrak
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            DropArt();
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);

        // Zýplama Lazer Kontrolü
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Zýplama Ýþlemi
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Yakalandýysa karakteri olduðu yere çivile
        if (isCaught || hasEscaped)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);

        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        if (direction == Vector3.zero) return;

        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }

    public void SetGrip(float value)
    {
        grip = value;
    }

    // --- KUTU TAÞIMA VE OYUN MANTIÐI ---

    public bool IsCarrying()
    {
        return carriedArt != null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCaught || hasEscaped) return;

        // Çarptýðýmýz obje eser/kutu mu?
        ArtPiece art = other.GetComponent<ArtPiece>();
        if (art == null) return;
        if (IsCarrying()) return; // Zaten bir þey taþýyorsak alma
        if (!art.CanBePickedUp()) return;

        carriedArt = art;
        art.PickUp(carryPoint != null ? carryPoint : transform);
        Debug.Log($"[Hýrsýz] {art.name} kafaya alýndý!");
    }

    public void DropArt()
    {
        if (!IsCarrying()) return;

        carriedArt.Drop();
        carriedArt = null;
    }

    public void SpotByCamera()
    {
        if (isCaught || hasEscaped) return;

        if (IsCarrying())
        {
            Debug.LogWarning("[Güvenlik Kamerasý] ALARM! Eser kafandan düþtü!");
            carriedArt.Drop();
            carriedArt = null;
        }
    }

    public ArtPiece HandOverArt()
    {
        if (!IsCarrying()) return null;

        ArtPiece delivered = carriedArt;
        carriedArt = null;
        totalDeliveredArt++;
        Debug.Log($"<color=cyan>[Müze Çýkýþý] Eser kaçýrýldý! ({totalDeliveredArt}/{totalArtInLevel})</color>");
        return delivered;
    }
}