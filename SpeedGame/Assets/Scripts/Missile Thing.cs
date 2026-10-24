using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class MissileThing : MonoBehaviour
{
    public GameObject missile;
    public Transform shootDirection;

    public int ammoCount = 1;
    public int maxAmmoCount = 5;

    public TextMeshProUGUI ammoText;

    private void Start()
    {
        StartCoroutine(RefreshAmmo());
    }

    private void Update()
    {
        ammoCount = Mathf.Clamp(ammoCount, 0, maxAmmoCount);

        ammoText.text = "Missile: " + ammoCount + "/" + maxAmmoCount;
    }

    void OnShoot(InputValue input)
    {
        if(ammoCount > 0)
        {
            Instantiate(missile, shootDirection.position, transform.rotation);
            ammoCount--;
        }
    }

    IEnumerator RefreshAmmo()
    {
        yield return new WaitForSeconds(4);

        if(ammoCount < maxAmmoCount) ammoCount++;

        StartCoroutine(RefreshAmmo());
    }
}
