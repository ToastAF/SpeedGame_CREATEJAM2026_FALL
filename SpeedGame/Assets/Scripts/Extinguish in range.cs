using UnityEngine;

public class Extinguishinrange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Fire fire = other.transform.parent.gameObject.GetComponentInChildren<Fire>();

        //Debug.Log(fire);

        if(fire != null)
        {
            //Debug.Log("Fire Found!");
            fire.FireNoMore();
        }
    }
}
