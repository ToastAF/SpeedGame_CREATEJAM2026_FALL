using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownClock : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 180f;
    public bool startOnAwake = true;
    [Tooltip("Hvor mange sekunder fra nul begynder den at blive større og rød")]
    public float dangerTime = 20f;
    [Tooltip("Vis decimaler når der er færre sekunder end dette tilbage")]
    public float showDecimalsBelow = 20f;
    public UnityEvent onTimeUp;

    [Header("Growth near zero")]
    public float sizeIncreaseAtZero = 0.8f;      // 0.8 = 80% større ved nul
    public float growthSmoothTime = 0.25f;
    public Color dangerColor = new Color(1f, 0.1f, 0.1f);

    [Header("Tick punch (small jump)")]
    public float punchStrength = 3f;
    public float springStiffness = 220f;
    public float springDamping = 14f;

    [Header("Idle")]
    public float idleFloatHeight = 2f;
    public float idleFloatSpeed = 0.6f;

    float timeLeft;
    bool running;
    bool finished;

    float shownUrgency, shownUrgencyVel;
    float punch, punchVel;
    int lastWholeSecond;

    float baseFontSize;
    Color baseColor;
    Vector3 basePos;
    TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        baseFontSize = text.fontSize;
        baseColor = text.color;
        basePos = transform.localPosition;
        ResetTimer();
        running = startOnAwake;
    }

    public void StartTimer() { running = true; }
    public void PauseTimer() { running = false; }

    public void ResetTimer()
    {
        timeLeft = startTime;
        finished = false;
        lastWholeSecond = Mathf.CeilToInt(timeLeft);
        shownUrgency = 0f;
        shownUrgencyVel = 0f;
    }

    public void AddTime(float seconds)
    {
        timeLeft = Mathf.Max(0f, timeLeft + seconds);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (running && !finished)
        {
            timeLeft = Mathf.Max(0f, timeLeft - dt);
            if (timeLeft <= 0f)
            {
                finished = true;
                running = false;
                onTimeUp?.Invoke();
            }
        }

        // 0 = rolig, 1 = nul tid tilbage
        float urgency = dangerTime > 0f ? 1f - Mathf.Clamp01(timeLeft / dangerTime) : 0f;
        shownUrgency = Mathf.SmoothDamp(shownUrgency, urgency, ref shownUrgencyVel, growthSmoothTime);

        // Et lille "tick" hver gang et helt sekund går
        int whole = Mathf.CeilToInt(timeLeft);
        if (whole != lastWholeSecond)
        {
            lastWholeSecond = whole;
            punchVel += punchStrength;
        }

        // Damped spring til punch
        punchVel += (-springStiffness * punch - springDamping * punchVel) * dt;
        punch += punchVel * dt;

        UpdateText();

        // Størrelse: vokser mod nul + det lille hop
        float scale = 1f + shownUrgency * sizeIncreaseAtZero + punch * 0.5f;
        text.fontSize = baseFontSize * Mathf.Max(0.1f, scale);

        // Farve mod rød
        text.color = Color.Lerp(baseColor, dangerColor, shownUrgency);

        // Idle-svæv, ingen rystelse eller tilt
        float idle = Mathf.Sin(Time.time * idleFloatSpeed * Mathf.PI * 2f) * idleFloatHeight;
        transform.localPosition = basePos + new Vector3(0f, idle, 0f);
        transform.localRotation = Quaternion.identity;
    }

    void UpdateText()
    {
        if (timeLeft < showDecimalsBelow)
        {
            text.text = timeLeft.ToString("0.0");
        }
        else
        {
            int total = Mathf.CeilToInt(timeLeft);
            int minutes = total / 60;
            int seconds = total % 60;
            text.text = $"0{minutes:0}:{seconds:00}";
        }
    }
}