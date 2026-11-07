
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private Player player;

    void Update()
    {
        HealthUpdate(player.CurrentHealth, player.MaxHealth);
    }

    public void HealthUpdate(float actualHealth, float maxHealth)
    {
        fillBar.fillAmount = actualHealth / maxHealth;
    }
}

