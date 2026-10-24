using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 movementVector;
    public float turnSpeed = 1;

    public float moveSpeed = 1;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(movementVector.magnitude > 0.15f)
        {
            transform.Rotate(new Vector3(0, movementVector.x * turnSpeed * Time.deltaTime, 0));
        }

        rb.AddRelativeForce(Vector3.forward * moveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    void OnMove(InputValue input)
    {
        movementVector = input.Get<Vector2>();
    }
}
