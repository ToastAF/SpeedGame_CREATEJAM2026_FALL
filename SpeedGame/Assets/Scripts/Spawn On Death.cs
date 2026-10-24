using System.Collections;
using UnityEngine;

public class SpawnOnDeath : MonoBehaviour
{
    public GameObject explosionObject;
    public GameObject particles, directionObject;

    public float flySpeed = 10;
    public float selfDestructTime = 3;

    private void Start()
    {
        Vector3 dir = (transform.position - directionObject.transform.position).normalized;

        GetComponent<Rigidbody>().AddForce(-dir * flySpeed, ForceMode.Impulse);

        StartCoroutine(KillAfterTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(explosionObject != null) Instantiate(explosionObject, transform.position, Quaternion.identity);
        if(particles != null) Instantiate(particles, transform.position, Quaternion.identity);
       
        Destroy(gameObject);
    }

    IEnumerator KillAfterTime()
    {
        yield return new WaitForSeconds(selfDestructTime);

        if (explosionObject != null) Instantiate(explosionObject, transform.position, Quaternion.identity);
        if (particles != null) Instantiate(particles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
