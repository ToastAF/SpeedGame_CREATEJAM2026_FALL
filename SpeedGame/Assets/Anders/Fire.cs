using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Fire : MonoBehaviour
{
    public FireSpreaderController FireSpreaderController;

    private Material burnMaterial;

    public float value;

    public float Intensity;
    public Extinguish Extinguish;

    public ParticleSystem ParticleSystem;
    ParticleSystem.Particle[] particles;
    public float radius = 10f;
    public float randomness = 0.2f;
    public float pushStrength = 10f;
    public float maxSpeed = 20f;

    public int SpreadCount;

    private void Start()
    {
        burnMaterial = GetComponentInParent<MeshRenderer>().materials[1];

        particles = new ParticleSystem.Particle[ParticleSystem.main.maxParticles];
    }

    private void LateUpdate()
    {
        if (Extinguish != null)
        {
            int count = ParticleSystem.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                Vector3 worldPos = particles[i].position;

                Vector3 away = worldPos - Extinguish.transform.position;
                float dist = away.magnitude;

                if (dist < radius && dist > 0.001f)
                {
                    float influence = 1f - (dist / radius);

                    Vector3 dir = away.normalized;
                    dir += Random.insideUnitSphere * randomness;
                    dir.Normalize();

                    Vector3 force = dir * pushStrength * influence * Time.deltaTime;

                    particles[i].velocity += force;

                    // Begræns hastigheden
                    if (particles[i].velocity.magnitude > maxSpeed)
                        particles[i].velocity = particles[i].velocity.normalized * maxSpeed;
                }
            }

            ParticleSystem.SetParticles(particles, count);
        }
    }

    private void Update()
    {
        if (Extinguish == null)
        {
            if (Intensity < 1f)
            {
                Intensity += Time.deltaTime * 0.01f;
            }
        }
        else
        {
            Intensity -= Time.deltaTime * 0.1f;

            if (Intensity < 0f)
            {
                Destroy(gameObject);
            }
        }

        var emission = ParticleSystem.emission;

        emission.rateOverTime = Mathf.Lerp(1f, 10, Intensity);



        if (value < 1f)
        {
            value += Time.deltaTime * Intensity * 0.05f;

            if (value > .07f && SpreadCount == 0)
            {
                CastFire();
                SpreadCount += 1;
            }

            if (value > .3f && SpreadCount == 1)
            {
                CastFire();
                SpreadCount += 1;
            }

            if (value > .75f && SpreadCount == 2)
            {
                CastFire();
                SpreadCount += 1;
            }
        }
        else
        {
            if (transform.parent != null)
            {
                GameObject parent = transform.parent.gameObject;
                transform.parent = null;
                Destroy(parent);
            }
        }

        burnMaterial.SetFloat("_BurnAmount", Mathf.Lerp(0.08f, 1, value));
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
