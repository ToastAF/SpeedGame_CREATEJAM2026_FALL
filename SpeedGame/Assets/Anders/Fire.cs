using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using System.Linq;

public class Fire : MonoBehaviour
{
    public FireSpreaderController FireSpreaderController;

    public Material burnMaterial;

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

    public List<MeshRenderer> allRenderers;
    public List<MeshRenderer> childRenderers;

    public List<Material> activeBurnMaterials;

    public bool HasBeenExstrichs;
    public float DestoryTimer;
    public GameObject ExstrichsParticel;

    public AudioSource fireDie;


    private void Start()
    {
        if (GetComponent<MeshRenderer>() != null) allRenderers.Add(GetComponent<MeshRenderer>());

        childRenderers = transform.parent.GetComponentsInChildren<MeshRenderer>().ToList();
        if(childRenderers != null)
        {
            //Debug.Log("Children count: " + childRenderers.Count);
            if (childRenderers.Count > 0) 
            {
                foreach(MeshRenderer child in childRenderers)
                {
                    allRenderers.Add(child);
                }
            }

        }
        

        foreach (MeshRenderer rend in allRenderers)
        {
            Material[] oldMats = rend.materials;

            string burnName = burnMaterial.name + " (Instance)";
            if (!rend.materials.Any(m => m.name == burnName))
            {
                Material[] newMats = new Material[oldMats.Length + 1];

                for (int i = 0; i < oldMats.Length; i++)
                    newMats[i] = oldMats[i];

                newMats[newMats.Length - 1] = burnMaterial;

                rend.materials = newMats;
            }
            else
            {
                Debug.Log("Burn Material: " + FindMaterial(oldMats, burnName));

                float burnValue = FindMaterial(oldMats, burnName).GetFloat("_BurnAmount");
                value = burnValue;
            }

            activeBurnMaterials.Add(rend.materials[1]);
            //burnMaterial = rend.materials[1];
        }
        //Debug.Log("All renderes count: " + allRenderers.Count);


        particles = new ParticleSystem.Particle[ParticleSystem.main.maxParticles];

        if (GetComponentInParent<FlameAble>() == null)
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponentInParent<FlameAble>().Fire = this;
        } 
    }

    private void LateUpdate()
    {
        if (HasBeenExstrichs)
            { return; }

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

                    // Begr�ns hastigheden
                    if (particles[i].velocity.magnitude > maxSpeed)
                        particles[i].velocity = particles[i].velocity.normalized * maxSpeed;
                }
            }

            ParticleSystem.SetParticles(particles, count);
        }
    }

    private void Update()
    {
        if (HasBeenExstrichs)
        {
            if (DestoryTimer > 10)
            {
                Destroy(gameObject);
            }

            DestoryTimer += Time.deltaTime;

            return;
        }


        if (Extinguish == null)
        {
            if (Intensity < 1f)
            {
                Intensity += Time.deltaTime * 0.01f;
            }
        }
        else
        {
            Intensity =  0;

            if (Intensity <= 0f)
            {
                FireNoMore();
            }
        }

        var emission = ParticleSystem.emission;

        emission.rateOverTime = Mathf.Lerp(3f, 15, Intensity);



        if (value < 1f)
        {
            value += Time.deltaTime * Intensity * 0.05f;

            if (value > .07f && SpreadCount == 0)
            {
                SpreadCount += 1;
            }

            if (value > .3f && SpreadCount == 1)
            {
                SpreadCount += 1;
            }

            if (value > .75f && SpreadCount == 2)
            {
                SpreadCount += 1;
            }
        }
        else
        {
            if (transform.parent != null)
            {
                GameObject parent = transform.parent.gameObject;
                Destroy(parent);
            }
        }
        foreach(Material mat in activeBurnMaterials)
        {
            mat.SetFloat("_BurnAmount", Mathf.Lerp(0.08f, 1, value));
        }
    }

    public void FireNoMore()
    {
        FindAnyObjectByType<ScorerKeeper>().AddCombo(20);
        HasBeenExstrichs = true;
        Destroy(gameObject.transform.GetChild(0).gameObject);
        Instantiate(ExstrichsParticel, transform.position, Quaternion.identity, this.transform);

        fireDie.Play();
    }

    public void CastFire()
    {
        Debug.Log("CastFire called");

        List<GameObject> list = new List<GameObject>();

        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var col in hits)
        {
            if (col.GetComponent<FlameAble>() == null) continue;
            if (col.GetComponentInChildren<Fire>() != null) continue;
            list.Add(col.gameObject);
        }

        if (list.Count == 0)
        {
            return;
        }

        GameObject gm = list[Random.Range(0, list.Count)];
        FireSpreaderController.SpawnFire(gm);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }

    Material FindMaterial(Material[] mats, string name)
    {
        foreach (Material m in mats)
        {
            if (m.name.StartsWith(name))
                return m;
        }
        return null;
    }
}
