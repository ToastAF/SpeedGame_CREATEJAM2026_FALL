using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float force = 25;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (rb.transform.position - transform.position).normalized;

            rb.AddForce(direction * force, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}