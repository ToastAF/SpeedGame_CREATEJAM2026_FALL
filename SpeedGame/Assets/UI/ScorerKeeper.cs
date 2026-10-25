using TMPro;
using UnityEngine;

public class ScorerKeeper : MonoBehaviour
{
    [Header("Combo")]
    public int maxCombo = 15;
    public float comboDecayDelay = 1.5f;
    public float comboDecayPerSecond = 2.5f;

    [Header("Growth per combo")]
    public float sizeIncreasePerCombo = 0.025f;
    public float growthSmoothTime = 1f;
    public Color hotColor = new Color(1f, 0.75f, 0.2f);

    [Header("Move down per combo")]
    public float downStepPerCombo = .2f;

    [Header("Hit punch (scale)")]
    public float punchStrength = 4f;
    public float punchStrengthAtMax = 6f;
    public float springStiffness = 220f;
    public float springDamping = 14f;

    [Header("Hit tilt (rotation)")]
    public float tiltStrength = 100f;
    public float tiltStiffness = 260f;
    public float tiltDamping = 16f;

    [Header("Idle")]
    public float idleFloatHeight = 2f;
    public float idleFloatSpeed = 0.6f;

    [Header("Testing")]
    public bool autoAddComboForTesting = true;

    float comboValue;
    float lastHitTime;

    float shownCombo, shownComboVel;

    float punch, punchVel;
    float tilt, tiltVel;
    int tiltDir = 1;

    float baseFontSize;
    Color baseColor;
    Vector3 basePos;
    public int score;
    TMP_Text text;
    float timer;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        baseFontSize = text.fontSize;
        baseColor = text.color;
        basePos = transform.localPosition;
    }

    public void AddCombo(int points)
    {
        comboValue = Mathf.Min(comboValue + 1f, maxCombo);
        lastHitTime = Time.time;

        float t = Mathf.Clamp01(comboValue / maxCombo);
        punchVel += Mathf.Lerp(punchStrength, punchStrengthAtMax, t);

        tiltDir = -tiltDir;
        tiltVel += tiltStrength * tiltDir;

        score += points;
    }

    public void ResetCombo()
    {
        comboValue = 0f;
    }

    void Update()
    {
        text.text = score.ToString();

        float dt = Time.deltaTime;

        if (autoAddComboForTesting)
        {
            timer += dt;
            if (timer > 0.7f)
            {
                AddCombo(10);
                timer = 0f;
            }
            if (comboValue >= maxCombo)
                autoAddComboForTesting = false;
        }

        // Combo drains smoothly after a short delay
        if (comboValue > 0f && Time.time - lastHitTime > comboDecayDelay)
            comboValue = Mathf.Max(0f, comboValue - comboDecayPerSecond * dt);

        // Den viste combo glider mod den rigtige (giver blød vækst + blød nedadbevægelse)
        shownCombo = Mathf.SmoothDamp(shownCombo, comboValue, ref shownComboVel, growthSmoothTime);

        // Damped springs
        punchVel += (-springStiffness * punch - springDamping * punchVel) * dt;
        punch += punchVel * dt;

        tiltVel += (-tiltStiffness * tilt - tiltDamping * tiltVel) * dt;
        tilt += tiltVel * dt;

        // Størrelse: vokser lineært for hver combo
        float scale = 1f + shownCombo * sizeIncreasePerCombo + punch * 0.5f;
        text.fontSize = baseFontSize * Mathf.Max(0.1f, scale);

        // Farve bygger op mod hotColor
        float colorT = Mathf.Clamp01(shownCombo / maxCombo);
        text.color = Color.Lerp(baseColor, hotColor, colorT);

        // Position: rykker nedad for hver combo + idle-svæv
        float idle = Mathf.Sin(Time.time * idleFloatSpeed * Mathf.PI * 2f) * idleFloatHeight;
        float down = -shownCombo * downStepPerCombo;
        transform.localPosition = basePos + Vector3.up * (down + idle);
        transform.localRotation = Quaternion.Euler(0f, 0f, tilt);
    }
}