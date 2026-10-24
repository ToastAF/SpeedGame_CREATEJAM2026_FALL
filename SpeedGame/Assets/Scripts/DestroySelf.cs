using System.Collections;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float timeToDie = 3;

    void Start()
    {
        StartCoroutine(DestroyObject(timeToDie));
    }


    IEnumerator DestroyObject(float x)
    {
        yield return new WaitForSeconds(timeToDie);

        Destroy(gameObject);
    }
}
