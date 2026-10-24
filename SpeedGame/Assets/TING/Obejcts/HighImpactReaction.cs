using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HighImpactReaction : MonoBehaviour
{
    [SerializeField] private float impactThreshold = 8f;

    public List<GameObject> objectsToActivateOnImpact;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= impactThreshold)
        {
            foreach (var item in objectsToActivateOnImpact)
            {
                item.GetComponent<BoxCollider>().enabled = true;
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.transform.parent = null;
            }
        }
    }
}