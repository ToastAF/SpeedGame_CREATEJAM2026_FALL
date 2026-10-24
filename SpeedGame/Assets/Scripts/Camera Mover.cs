using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    public GameObject player;
    Camera cam;
    Rigidbody rb;
    public Vector3 offset;

    Vector3 target;

    public float moveSpeed = 1;

    public float minFOV, maxFOV, maxMoveSpeed;

    public GameObject rotateObject;
    public float rotateSpeed = 1;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = player.GetComponent<Rigidbody>();
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetAxis("Mouse X") > 0.1f) // Rotate camera around player
        {
            //Debug.Log("Right!");
            rotateObject.transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }
        else if(Input.GetAxis("Mouse X") < -0.1f)
        {
            //Debug.Log("Left!");
            rotateObject.transform.Rotate(new Vector3(0, -rotateSpeed * Time.deltaTime, 0));
        }
        //Debug.Log(Input.GetAxis("Mouse X"));
        transform.LookAt(player.transform);

        //target = player.transform.position + offset;

        //transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //Debug.Log(rb.linearVelocity.magnitude);

        float t = rb.linearVelocity.magnitude / maxMoveSpeed;
        //t = t * t * (3 - 2 * t);
        cam.fieldOfView = Mathf.Lerp(minFOV, maxFOV, Mathf.SmoothStep(0, 1, t));
    }
}
