using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 4f;

    // which layers count as ground: set it to Ground!
    [SerializeField] private LayerMask groundLayers;

    private Rigidbody rb;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        // only in the frame Space / South button goes down
        if (!jumpAction.WasPressedThisFrame()) return;
        if (!IsGrounded()) return;   // no jumping in the air

        // one short kick upwards
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // is there Ground right under our feet?
    bool IsGrounded()
    {
        // from our middle, 1.1 m down: half our height + 0.1
        return Physics.Raycast(transform.position, Vector3.down,
                               1.1f, groundLayers);
    }
}
