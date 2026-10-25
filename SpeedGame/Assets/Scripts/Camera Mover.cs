using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public GameObject player;
    Camera cam;
    Rigidbody rb;

    public float minFOV, maxFOV, maxMoveSpeed;
    [SerializeField] private float fovSmoothTime = 0.3f;
    private float fovVelocity;

    public GameObject rotateObject;
    public float rotateSpeed = 3f;          
    [SerializeField] private float rotateSmoothTime = 0.08f;

    [SerializeField] private float lookSmoothTime = 0.1f;

    float targetYaw, currentYaw, yawVelocity;
    Vector3 lookPoint, lookVelocity;

    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;

        rb = player.GetComponent<Rigidbody>();
        cam = GetComponent<Camera>();

        targetYaw = currentYaw = rotateObject.transform.eulerAngles.y;
        lookPoint = player.transform.position;
    }

    void LateUpdate()
    {
        // Rotation: direkte fra musen, ingen tærskel, ingen deltaTime
        targetYaw += Input.GetAxis("Mouse X") * rotateSpeed;
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVelocity, rotateSmoothTime);
        rotateObject.transform.rotation = Quaternion.Euler(0, currentYaw, 0);

        // Kig på et blødt, forsinket punkt i stedet for spillerens eksakte position
        lookPoint = Vector3.SmoothDamp(lookPoint, player.transform.position, ref lookVelocity, lookSmoothTime);
        transform.LookAt(lookPoint);

        // FOV
        float t = Mathf.Clamp01(rb.linearVelocity.magnitude / maxMoveSpeed);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, Mathf.SmoothStep(0, 1, t));
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFOV, ref fovVelocity, fovSmoothTime);
    }
}