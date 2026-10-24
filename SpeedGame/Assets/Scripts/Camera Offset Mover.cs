using UnityEngine;

public class CameraOffsetMover : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 20;

    void Update()
    {
        moveSpeed = player.GetComponent<Rigidbody>().linearVelocity.magnitude * 0.99f;

        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }
}
