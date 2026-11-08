using UnityEngine;

public class KeyIconUI : MonoBehaviour
{
    [SerializeField] GameObject keyIcon;
    Player player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.OnKeyChanged += UpdateIcon;
        UpdateIcon(player.HasKey);
    }

    void OnDestroy()
    {
        if (player != null) player.OnKeyChanged -= UpdateIcon;
    }

    void UpdateIcon(bool hasKey)
    {
        keyIcon.SetActive(hasKey);
    }
}