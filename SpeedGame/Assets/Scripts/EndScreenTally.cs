using UnityEngine;
using TMPro;

public class EndScreenTally : MonoBehaviour
{
    public bool endScreenThing = false;

    public float points;

    public TextMeshProUGUI bro;

    private void Awake()
    {

    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);


    }

    private void Update()
    {        
        if (endScreenThing)
        {
            bro = GameObject.FindGameObjectWithTag("scoreThing").GetComponent<TextMeshProUGUI>();
            endScreenThing = false;

            Cursor.lockState = CursorLockMode.None;
        }
        bro.text = points.ToString();
    }

}
