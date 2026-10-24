using UnityEngine;

public class Extinguish : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Fire fire = other.GetComponentInChildren<Fire>();

        if (fire != null)
        {
            fire.Extinguish = this;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Fire fire = other.GetComponentInChildren<Fire>();
        if (fire != null)
        {
            fire.Extinguish = null;
        }
    }
}
