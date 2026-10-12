using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;
    [SerializeField] private float moveSpeed = 6f;

    private Cargo carried;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (carryPoint == null)
        {
            Transform found = transform.Find("CarryPoint");
            if (found != null) carryPoint = found;
        }
    }

    void Update()
    {
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

        if (horizontal != 0f || vertical != 0f)
        {
            Vector3 move = new Vector3(horizontal, 0f, vertical).normalized * moveSpeed * Time.deltaTime;
            if (rb != null && !rb.isKinematic)
            {
                rb.MovePosition(rb.position + move);
            }
            else
            {
                transform.position += move;
            }
        }
    }

    public bool IsCarrying()
    {
        return carried != null;
    }

    void OnTriggerEnter(Collider other)
    {
        Cargo cargo = other.GetComponent<Cargo>();
        if (cargo == null) return;
        if (IsCarrying()) return;
        if (!cargo.CanBePickedUp()) return;

        carried = cargo;
        cargo.PickUp(carryPoint != null ? carryPoint : transform);
    }

    public void DropCargo()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
    }

    public Cargo HandOver()
    {
        Cargo cargo = carried;
        carried = null;
        return cargo;
    }
}
