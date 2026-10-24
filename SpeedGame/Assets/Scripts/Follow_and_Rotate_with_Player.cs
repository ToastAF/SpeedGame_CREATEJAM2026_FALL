using UnityEngine;

public class Follow_and_Rotate_with_Player : MonoBehaviour
{
    public GameObject player, target;
    public float rotateSpeed = 1;

    void FixedUpdate()
    {
        transform.position = player.transform.position;

        //Vector3 dir = new Vector3(target.transform.rotation.x, transform.rotation.y, target.transform.rotation.z);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, rotateSpeed * Time.deltaTime);

        //transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
    }
}
