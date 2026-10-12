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
    [SerializeField] private float gravity = 20f;

    [Header("Carrying")]
    [SerializeField] private Transform carryPoint;
    public ArtPiece carriedArt;

    [Header("Game Progress")]
    public int totalDeliveredArt = 0;
    public int totalArtInLevel = 5;
    public bool isCaught = false;
    public bool hasEscaped = false;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float verticalVelocity;

    void Awake()
    {
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

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
#endif

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime).normalized;
            moveDirection = inputDir * moveSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
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
