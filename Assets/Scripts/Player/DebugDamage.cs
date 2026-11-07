using UnityEngine;

public class DebugDamage : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] private float amount = 10f;

    void Update()
    {
        if (player == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            player.LoseHealth(amount);
            LogState("Damage");
        }

        if (Input.GetMouseButtonDown(1))
        {
            player.RestoreHealth(amount);
            LogState("Heal");
        }
    }

    private void LogState(string action)
    {
        string stress = StressManager.Instance != null
            ? StressManager.Instance.Stress01.ToString("F2")
            : "no StressManager";

        Debug.Log($"{action} -> health {player.CurrentHealth}/{player.MaxHealth}, stress {stress}");
    }
}