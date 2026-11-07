using System;
using UnityEngine;

public enum StressLevel { Calm, Uneasy, Stressed, Panic }

public class StressManager : MonoBehaviour
{
    public static StressManager Instance { get; private set; }

    [SerializeField] private Player player;

    [Header("Umbrales de nivel (estrés 0-1)")]
    [SerializeField] private float uneasyThreshold = 0.25f;
    [SerializeField] private float stressedThreshold = 0.5f;
    [SerializeField] private float panicThreshold = 0.75f;

    [Header("Curvas (X = estrés 0-1, Y = multiplicador)")]
    [SerializeField] private AnimationCurve jumpscareChanceCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve enemySpawnCurve = AnimationCurve.Linear(0, 1, 1, 2);

    // 0 = sin estrés (vida completa), 1 = estrés máximo (vida a 0)
    public float Stress01 { get; private set; }
    public StressLevel Level { get; private set; }

    // Valores ya listos para que los consuman otros scripts
    public float JumpscareChanceMultiplier => jumpscareChanceCurve.Evaluate(Stress01);
    public float EnemySpawnMultiplier => enemySpawnCurve.Evaluate(Stress01);

    public event Action<float> OnStressChanged;
    public event Action<StressLevel> OnLevelChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (player == null || player.MaxHealth <= 0f) return;

        float newStress = 1f - Mathf.Clamp01(player.CurrentHealth / player.MaxHealth);

        if (!Mathf.Approximately(newStress, Stress01))
        {
            Stress01 = newStress;
            OnStressChanged?.Invoke(Stress01);

            StressLevel newLevel = CalculateLevel(Stress01);
            if (newLevel != Level)
            {
                Level = newLevel;
                OnLevelChanged?.Invoke(Level);
            }
        }
    }

    private StressLevel CalculateLevel(float stress)
    {
        if (stress >= panicThreshold) return StressLevel.Panic;
        if (stress >= stressedThreshold) return StressLevel.Stressed;
        if (stress >= uneasyThreshold) return StressLevel.Uneasy;
        return StressLevel.Calm;
    }
}
