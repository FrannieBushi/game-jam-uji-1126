using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    
    [SerializeField] private Image fillBar;

    public void HealthUpdate(float actualHealth, float maxHealth)
    {
        fillBar.fillAmount = actualHealth / maxHealth;
    }


    void Start()
    {
        
    }

    void Update()
    {
        HealthUpdate(10,10);   
    }
}
