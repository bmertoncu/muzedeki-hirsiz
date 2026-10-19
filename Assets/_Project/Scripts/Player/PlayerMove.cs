using UnityEngine;
using UnityEngine.InputSystem;
 
public class PlayerMove : MonoBehaviour
{
    // the three knobs of movement, shown in the Inspector
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;
 
    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;   // where the input points
    private float grip = 1f;     // 1 = normal ground
 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }
 
    void Update()
    {
        // every frame: read WASD, arrows or the left stick
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
    }
    void FixedUpdate()
    {
        // speed we want, speed we have (not up/down)
        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);
 
        // get a little closer to the wanted speed
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
 
        // turn towards the way we are going
        if (direction == Vector3.zero) return;
        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }
 
    // the icy ramp calls this
    public void SetGrip(float value)
    {
        grip = value;
    }
}