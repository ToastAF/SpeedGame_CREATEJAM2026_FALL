using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireSpreaderController : MonoBehaviour
{
    public List<GameObject> FireStarters;

    public GameObject FirePrefab;

    public List<GameObject> ObejctsOnFire;

    public float SpreadFireDelay = 10f;

    private float _spreadFireTimer = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FlameAble[] flammables = FindObjectsOfType<FlameAble>();

        for (int i = 0; i < 30; i++)
        {
            FireStarters.Add(flammables[Random.Range(0, flammables.Length)].gameObject);
        }

        foreach (var item in FireStarters)
        {
            SpawnFire(item);
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

    public void SpreadFire()
    {
        CleanListUp();

        if (ObejctsOnFire.Count == 0)
        {
            return;
        }

        ObejctsOnFire[Random.Range(0, ObejctsOnFire.Count)].GetComponentInChildren<Fire>().CastFire();
    }

    public void SpawnFire(GameObject gameObject)
    {
        if (gameObject.GetComponentInChildren<Fire>() != null)
        {
            return;
        }

        GameObject fireGM = Instantiate(FirePrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity, gameObject.transform);

        MeshRenderer render = fireGM.GetComponentInParent<MeshRenderer>();

        Fire fire = fireGM.GetComponent<Fire>();
        fire.FireSpreaderController = this;

        ObejctsOnFire.Add(gameObject);
    }


    private void CleanListUp()
    {
        foreach (var item in ObejctsOnFire.ToArray())
        {
            if (item == null)
            {
                ObejctsOnFire.Remove(item);
            }
            else
            {
                if (item.GetComponentInChildren<Fire>() == null)
                {
                    ObejctsOnFire.Remove(item);
                }
            }
        }
    }
}