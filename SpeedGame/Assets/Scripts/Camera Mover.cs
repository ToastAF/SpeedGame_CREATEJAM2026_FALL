using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;

    Vector3 target;

    public float moveSpeed = 1;

    void Start()
    {
        
    }

    void Update()
    {
        target = player.transform.position + offset;

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }
}
