using UnityEngine;

public class Fire : MonoBehaviour
{
    public FireSpreaderController FireSpreaderController;

    private Material burnMaterial;

    private float value = 0f;

    private void Start()
    {
        burnMaterial = GetComponentInParent<MeshRenderer>().materials[1];
    }

    private void Update()
    {
        if (value < 1f)
        {
            value += Time.deltaTime * 0.01f;
        }
        burnMaterial.SetFloat("_BurnAmount", value);
    }

    public void CastFire()
    {
        Debug.Log("CastFire called");

        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var col in hits)
        {
            if (col.GetComponent<FlameAble>() == null) continue;
            if (col.GetComponentInChildren<Fire>() != null) continue;
            FireSpreaderController.CanGetFire.Add(col.gameObject);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
