using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Hareket ve Zıplama")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 1.1f;

    [Header("Kutu Taşıma (Carrying)")]
    [SerializeField] private Transform carryPoint;
    public ArtPiece carriedArt;

    [Header("Oyun İlerlemesi")]
    public int totalDeliveredArt = 0;
    public int totalArtInLevel = 4;
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
        if (InputSystem.actions != null)
        {
            moveAction = InputSystem.actions.FindAction("Move");
        }

        // Kutu taşıma noktasını otomatik bul (eğer atanmamışsa)
        if (carryPoint == null)
        {
            Transform found = transform.Find("CarryPoint");
            if (found != null) carryPoint = found;
        }
    }

    void Update()
    {
        // Yakalandıysa veya kaçtıysa hiçbir şey yapma
        if (isCaught || hasEscaped) return;

        // Klavyeden E tuşuna basılırsa eseri/kutuyu bırak
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            DropArt();
        }

        Vector2 input = Vector2.zero;
        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>();
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
        }
        else
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }

        direction = new Vector3(input.x, 0f, input.y).normalized;

        // Zıplama Lazer Kontrolü
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Zıplama İşlemi
        bool jumpPressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) || Input.GetKeyDown(KeyCode.Space);
        if (jumpPressed && isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Yakalandıysa karakteri olduğu yere çivile
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

    // --- KUTU TAŞIMA VE OYUN MANTIĞI ---

    public bool IsCarrying()
    {
        return carriedArt != null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCaught || hasEscaped) return;

        // Çarptığımız obje eser/kutu mu?
        ArtPiece art = other.GetComponent<ArtPiece>();
        if (art == null) return;
        if (IsCarrying()) return; // Zaten bir şey taşıyorsak alma
        if (!art.CanBePickedUp()) return;

        carriedArt = art;
        art.PickUp(carryPoint != null ? carryPoint : transform);
        Debug.Log($"[Hırsız] {art.name} kafaya alındı!");
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
            Debug.LogWarning("[Güvenlik Kamerası] ALARM! Eser kafandan düştü!");
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
        Debug.Log($"<color=cyan>[Müze Çıkışı] Eser kaçırıldı! ({totalDeliveredArt}/{totalArtInLevel})</color>");
        return delivered;
    }
}
