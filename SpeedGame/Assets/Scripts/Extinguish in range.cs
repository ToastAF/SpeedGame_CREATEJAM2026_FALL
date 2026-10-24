using UnityEngine;

public class Extinguishinrange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Fire fire = other.GetComponentInChildren<Fire>();

        if(fire != null)
        {
            Destroy(fire.gameObject);
        }
    }
}
