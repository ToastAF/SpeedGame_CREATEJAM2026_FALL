using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float force = 10;

    public FireSpreaderController FireSpreaderController;

    public bool SpreadsFire;

    private float Timer = .2f;

    private void Start()
    {
        FireSpreaderController = FindObjectOfType<FireSpreaderController>();
    }

    private void Update()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (rb.transform.position - transform.position).normalized;

            rb.AddForce(direction * force, ForceMode.Impulse);

            if (SpreadsFire)
            {
                FireSpreaderController.SpawnFire(other.gameObject);
            }
        }
    }
}