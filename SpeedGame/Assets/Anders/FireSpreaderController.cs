using System.Collections.Generic;
using UnityEngine;

public class FireSpreaderController : MonoBehaviour
{
    public List<GameObject> FireStarters;

    public GameObject FirePrefab;

    public List<GameObject> CanGetFire;

    public float SpreadFireDelay = 10f;

    private float _spreadFireTimer = 0f;

    public Material BurnMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var item in FireStarters)
        {
            GameObject fireGM = Instantiate(FirePrefab, new Vector3(item.transform.position.x, item.transform.position.y, item.transform.position.z), Quaternion.identity, item.transform);

            MeshRenderer render = fireGM.GetComponentInParent<MeshRenderer>();

            Material[] oldMats = render.materials;
            Material[] newMats = new Material[oldMats.Length + 1];

            for (int i = 0; i < oldMats.Length; i++)
                newMats[i] = oldMats[i];

            newMats[newMats.Length - 1] = BurnMaterial;

            render.materials = newMats;

            Fire fire = fireGM.GetComponent<Fire>();
            fire.FireSpreaderController = this;

            fire.CastFire();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_spreadFireTimer > 0)
        {
            _spreadFireTimer -= Time.deltaTime;
        }
        else
        {
            SpreadFire();
            _spreadFireTimer = SpreadFireDelay;
        }
    }

    private void SpreadFire()
    {
        CleanListUp();

        if (CanGetFire.Count == 0)
        {
            return;
        }


        GameObject gameObject = CanGetFire[Random.Range(0, CanGetFire.Count)];

        GameObject fireGM = Instantiate(FirePrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity, gameObject.transform);

        MeshRenderer render = fireGM.GetComponentInParent<MeshRenderer>();

        Material[] oldMats = render.materials;
        Material[] newMats = new Material[oldMats.Length + 1];

        for (int i = 0; i < oldMats.Length; i++)
            newMats[i] = oldMats[i];

        newMats[newMats.Length - 1] = BurnMaterial;

        render.materials = newMats;

        Fire fire = fireGM.GetComponent<Fire>();
        fire.FireSpreaderController = this;
        

        fire.CastFire();
    }

    private void CleanListUp()
    {
        foreach (var item in CanGetFire.ToArray())
        {
            if (item == null)
            {
                CanGetFire.Remove(item);
            }

            if (item.GetComponentInChildren<Fire>() != null)
            {
                CanGetFire.Remove(item);
            }
        }
    }
}