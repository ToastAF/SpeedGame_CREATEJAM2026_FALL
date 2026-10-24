using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 movementVector;
    public float turnSpeed_input = 1;
    public float turnSpeed = 1;

    public float moveSpeed = 1;
    Rigidbody rb;

    public Transform moveDirectionObject; //This is on the "Player - Rotation Move Object" object

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(movementVector.magnitude > 0.15f)
        {
            transform.Rotate(new Vector3(0, movementVector.x * turnSpeed_input * Time.deltaTime, 0));
        }

        //Vector3 targetDir = new Vector3(moveDirectionObject.position.x, transform.position.y, moveDirectionObject.position.z);
        //Vector3 moveDir = (targetDir - transform.position).normalized;

        Vector3 targetDir = transform.position - moveDirectionObject.position;

        targetDir.y = 0;
        targetDir = targetDir.normalized;

        rb.AddForce(targetDir * -moveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    void OnMove(InputValue input)
    {
        movementVector = input.Get<Vector2>();
    }
}
