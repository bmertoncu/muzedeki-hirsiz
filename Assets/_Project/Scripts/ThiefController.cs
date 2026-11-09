using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class ThiefController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 14f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float jumpForce = 6.5f;    // Normal zıplama
    [SerializeField] private float lowJumpForce = 0.5f; // Ağır eşya zıplaması
    [SerializeField] private float gravity = 20f;

    [Header("Carrying")]
    [SerializeField] private Transform carryPoint;
    public ArtPiece carriedArt;

    [Header("Ağırlık Sistemi")]
    public int hirsizAgirligi = 1; // Hırsızın temel ağırlığı

    [Header("Game Progress")]
    public int totalDeliveredArt = 0;
    public int totalArtInLevel = 4;
    public bool isCaught = false;
    public bool hasEscaped = false;

    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 currentVelocity;
    private float grip = 1f;
    private float verticalVelocity;

    void Awake()
    {
        Application.targetFrameRate = 60;
        controller = GetComponent<CharacterController>();
        if (carryPoint == null)
        {
            Transform found = transform.Find("CarryPoint");
            if (found != null) carryPoint = found;
        }
    }

    void Update()
    {
        if (isCaught || hasEscaped) return;

        float horizontal = 0f;
        float vertical = 0f;
        bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) DropArt();
            if (Keyboard.current.spaceKey.wasPressedThisFrame) jumpPressed = true;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (horizontal == 0f && vertical == 0f)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        if (Input.GetKeyDown(KeyCode.E)) DropArt();
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;
#endif

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime).normalized;
        }

        Vector3 wanted = inputDir * moveSpeed;
        currentVelocity = Vector3.MoveTowards(currentVelocity, wanted, acceleration * grip * Time.deltaTime);
        moveDirection = currentVelocity;

        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;
            if (jumpPressed)
            {
                verticalVelocity = GetCurrentJumpForce();
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private float GetCurrentJumpForce()
    {
        int kutuAgirligi = 0;

        // Eşya taşıyorsak onun ağırlığını al
        if (carriedArt != null)
        {
            EsyaOzellikleri esya = carriedArt.GetComponent<EsyaOzellikleri>();
            if (esya != null)
            {
                kutuAgirligi = esya.agirlik;
            }
        }

        // İSTEDİĞİN MANTIK: Hırsız ağırlığı + Kutu ağırlığı
        int toplamAgirlik = hirsizAgirligi + kutuAgirligi;

        // 4 ve üzeriyse alçak zıpla (Örn: Hırsız 1 + Turuncu Kutu 3 = 4)
        if (toplamAgirlik >= 4)
        {
            return lowJumpForce;
        }

        // 4'ten küçükse normal zıpla (Örn: Hırsız 1 + Turkuaz Kutu 1 = 2)
        return jumpForce;
    }

    public bool IsCarrying()
    {
        return carriedArt != null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCaught || hasEscaped) return;

        ArtPiece art = other.GetComponent<ArtPiece>();
        if (art == null) return;
        if (IsCarrying()) return;
        if (!art.CanBePickedUp()) return;

        carriedArt = art;
        art.PickUp(carryPoint != null ? carryPoint : transform);
        Debug.Log($"[Hırsız] {art.name} kafaya alındı!");
    }

    public void SetGrip(float value)
    {
        grip = value;
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