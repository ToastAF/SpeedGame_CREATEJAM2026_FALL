using UnityEngine;
using UnityEngine.InputSystem;


public class MissileThing : MonoBehaviour
{
    public GameObject missile;
    public Transform shootDirection;

    public int ammoCount = 1;
    public int maxAmmoCount = 5;

    private void Update()
    {
        ammoCount = Mathf.Clamp(ammoCount, 0, ammoCount);
    }

    void OnShoot(InputValue input)
    {
        if(ammoCount > 0)
        {
            Instantiate(missile, shootDirection.position, transform.rotation);
            ammoCount--;
        }
    }
}
