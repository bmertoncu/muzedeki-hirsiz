using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Camera))]
public class CameraOcclusionController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Target to follow and keep in sight (usually Hirsiz_Karakter).")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.1f, 0f);

    [Header("Default Overview View")]
    [Tooltip("Default stationary position when character is in clear view.")]
    [SerializeField] private Vector3 defaultPosition = new Vector3(0f, 26f, -22f);
    [Tooltip("Default overview rotation (Euler angles).")]
    [SerializeField] private Vector3 defaultEulerAngles = new Vector3(55f, 0f, 0f);
    [SerializeField] private float defaultFOV = 60f;

    [Header("Occluded (High-Angle) View")]
    [Tooltip("Height above target when occluded behind objects.")]
    [SerializeField] private float occludedHeight = 24f;
    [Tooltip("South offset (distance behind target) when occluded.")]
    [SerializeField] private float occludedDistanceZ = 4.5f;
    [Tooltip("Pitch angle override for occluded view (if using fixed pitch). Set to 0 to look directly at target.")]
    [SerializeField] private float occludedPitchAngle = 78f;
    [SerializeField] private float occludedFOV = 58f;

    [Header("Transition Settings")]
    [SerializeField] private float positionSmoothTime = 0.28f;
    [SerializeField] private float rotationSmoothSpeed = 4.5f;
    [SerializeField] private float fovSmoothSpeed = 3.5f;
    [Tooltip("Delay in seconds before returning to default view after occlusion clears (prevents jitter).")]
    [SerializeField] private float recoveryDelay = 0.35f;

    [Header("Room Bounds (for camera position)")]
    [SerializeField] private Vector2 xBounds = new Vector2(-12f, 12f);
    [SerializeField] private Vector2 zBounds = new Vector2(-20f, 8f);

    [Header("Occlusion Detection")]
    [Tooltip("Radius for sphere casting to account for character volume.")]
    [SerializeField] private float checkRadius = 0.35f;

    [Header("Manual View Toggle")]
    [Tooltip("Allow pressing 'C' to toggle between overview and high-angle view manually.")]
    [SerializeField] private bool allowManualToggle = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.C;

    [Header("Debug / Status")]
    [SerializeField] private bool isOccluded = false;
    [SerializeField] private bool manualViewActive = false;

    private Camera cam;
    private Vector3 positionVelocity;
    private float lastOcclusionTime = -999f;
    private Quaternion defaultRotation;

    void Awake()
    {
        cam = GetComponent<Camera>();
        defaultRotation = Quaternion.Euler(defaultEulerAngles);

        // Auto-detect target if not linked
        if (target == null)
        {
            FindTarget();
        }
    }

    void Start()
    {
        if (target == null)
        {
            FindTarget();
        }
    }

    private void FindTarget()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            return;
        }

        ThiefController tc = FindObjectOfType<ThiefController>();
        if (tc != null)
        {
            target = tc.transform;
            return;
        }

        GameObject byName = GameObject.Find("Hirsiz_Karakter");
        if (byName != null)
        {
            target = byName.transform;
        }
    }

    void Update()
    {
        HandleManualInput();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindTarget();
            if (target == null) return;
        }

        // Check if hero is occluded from the default overview perspective or current perspective
        bool occludedFromDefault = CheckOcclusion(defaultPosition);
        bool occludedFromCurrent = CheckOcclusion(transform.position);

        if (occludedFromDefault || occludedFromCurrent)
        {
            lastOcclusionTime = Time.time;
            isOccluded = true;
        }
        else
        {
            if (Time.time - lastOcclusionTime > recoveryDelay)
            {
                isOccluded = false;
            }
        }

        bool shouldUseHighAngle = isOccluded || manualViewActive;

        Vector3 desiredPosition;
        Quaternion desiredRotation;
        float desiredFOV;

        if (shouldUseHighAngle)
        {
            // Compute high-angle pose focused on the hero
            Vector3 heroPos = target.position + targetOffset;
            float camX = Mathf.Clamp(target.position.x, xBounds.x, xBounds.y);
            float camZ = Mathf.Clamp(target.position.z - occludedDistanceZ, zBounds.x, zBounds.y);
            float camY = target.position.y + occludedHeight;
            desiredPosition = new Vector3(camX, camY, camZ);

            if (occludedPitchAngle > 0f)
            {
                // Dynamic look with prescribed steep pitch
                Vector3 lookDir = heroPos - desiredPosition;
                float yaw = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
                desiredRotation = Quaternion.Euler(occludedPitchAngle, yaw, 0f);
            }
            else
            {
                desiredRotation = Quaternion.LookRotation(heroPos - desiredPosition);
            }

            desiredFOV = occludedFOV;
        }
        else
        {
            // Default museum overview pose
            desiredPosition = defaultPosition;
            desiredRotation = defaultRotation;
            desiredFOV = defaultFOV;
        }

        // Smooth position damping (ease in / ease out)
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, positionSmoothTime);

        // Smooth rotation slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);

        // Smooth FOV
        if (cam != null && !Mathf.Approximately(cam.fieldOfView, desiredFOV))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFOV, fovSmoothSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Checks if obstacles block line of sight between vantage point and the target hero.
    /// Checks head, chest, and lower body so occlusion is caught immediately when stepping behind objects.
    /// </summary>
    private bool CheckOcclusion(Vector3 fromPosition)
    {
        if (target == null) return false;

        Vector3 headPoint = target.position + new Vector3(0f, 1.7f, 0f);
        Vector3 chestPoint = target.position + targetOffset;
        Vector3 lowerPoint = target.position + new Vector3(0f, 0.45f, 0f);

        return IsLineBlocked(fromPosition, chestPoint) ||
               IsLineBlocked(fromPosition, headPoint) ||
               IsLineBlocked(fromPosition, lowerPoint);
    }

    private bool IsLineBlocked(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        if (distance < 0.01f) return false;

        Ray ray = new Ray(from, direction.normalized);
        RaycastHit[] hits = (checkRadius > 0.01f)
            ? Physics.SphereCastAll(ray, checkRadius, distance, ~0, QueryTriggerInteraction.Ignore)
            : Physics.RaycastAll(ray, distance, ~0, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Collider col = hit.collider;
            if (col == null) continue;

            // Ignore target itself and its children
            if (col.transform == target || col.transform.IsChildOf(target)) continue;

            // Ignore floor
            if (col.CompareTag("Zemin") || col.gameObject.name.Contains("Zemin")) continue;

            // Ignore enemy patroller / guard (we only care about architectural obstacles like walls and partitions)
            if (col.GetComponentInParent<Patroller>() != null) continue;

            // Ignore art pieces on pedestals
            if (col.gameObject.layer == LayerMask.NameToLayer("ArtPiece") || col.GetComponentInParent<ArtPiece>() != null) continue;

            // If we hit an obstacle closer than target distance, line of sight is blocked
            if (hit.distance < distance - 0.2f)
            {
                return true;
            }
        }

        return false;
    }

    private void HandleManualInput()
    {
        if (!allowManualToggle) return;

        bool togglePressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            togglePressed = true;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(toggleKey))
        {
            togglePressed = true;
        }
#endif

        if (togglePressed)
        {
            manualViewActive = !manualViewActive;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = isOccluded ? Color.red : Color.green;
        Vector3 heroPos = target.position + targetOffset;
        Gizmos.DrawLine(transform.position, heroPos);
        Gizmos.DrawWireSphere(heroPos, 0.3f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(defaultPosition, 0.5f);
    }
}
