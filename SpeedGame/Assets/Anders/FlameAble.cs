using UnityEngine;

public class FlameAble : MonoBehaviour
{
    [Header("Explosion")]
    public bool CanExplode;
    public float HeatCapbility;
    public float ExplosionForce;

    public GameObject ExplosionPrefab;
    public GameObject ExplosionParticels;

    public Fire Fire;

    // Update is called once per frame
    void Update()
    {
        if (Fire == null)
        {
            return;
        }

        if (CanExplode == true)
        {
            if (Fire.Intensity >= HeatCapbility)
            {
                Explosion explosion = Instantiate (ExplosionPrefab, transform.position, Quaternion.identity).GetComponent<Explosion>();
                Instantiate(ExplosionParticels, transform.position, Quaternion.identity);
                explosion.force = ExplosionForce;
                explosion.SpreadsFire = true;

                Destroy(gameObject);
            }
        }
    }
}
