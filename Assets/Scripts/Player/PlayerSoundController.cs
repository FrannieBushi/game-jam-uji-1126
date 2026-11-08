using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Player player;

    [Header("Pasos")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.4f;
    [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.6f;
    [SerializeField] private Vector2 stepPitchRange = new Vector2(0.9f, 1.1f);

    [Header("Latido")]
    [SerializeField] private AudioClip heartbeatClip;
    [Tooltip("Por debajo de este % de vida empieza a sonar el latido")]
    [SerializeField, Range(0f, 1f)] private float heartbeatStartThreshold = 0.5f;
    [SerializeField] private float slowestInterval = 1.2f;   // justo al llegar al umbral
    [SerializeField] private float fastestInterval = 0.35f;  // con vida casi a 0
    [SerializeField, Range(0f, 1f)] private float minHeartbeatVolume = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxHeartbeatVolume = 1f;
    [SerializeField] private Vector2 heartbeatPitchRange = new Vector2(0.95f, 1.15f);

    [Header("Daño")]
    [SerializeField] private AudioClip[] damageClips;
    [SerializeField, Range(0f, 1f)] private float damageVolume = 0.9f;

    private AudioSource sfxSource;
    private float stepTimer;
    private float heartbeatTimer;

    void Awake()
    {
        player = GetComponent<Player>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f; // 2D
    }

    void OnEnable()
    {
        if (player != null) player.OnDamaged += HandleDamage;
    }

    void OnDisable()
    {
        if (player != null) player.OnDamaged -= HandleDamage;
    }

    void Update()
    {
        if (player == null) return;
        UpdateFootsteps();
        UpdateHeartbeat();
    }

    private void UpdateFootsteps()
    {
        if (!player.IsMoving)
        {
            stepTimer = 0f; // al empezar a moverse suena el primer paso al instante
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            PlayRandom(footstepClips, footstepVolume, stepPitchRange);
            stepTimer = stepInterval;
        }
    }

    private void UpdateHeartbeat()
    {
        float health01 = player.CurrentHealth / player.MaxHealth;

        // Sin latido si vida alta o si ya está muerto
        if (health01 >= heartbeatStartThreshold || health01 <= 0f || heartbeatClip == null)
        {
            heartbeatTimer = 0f;
            return;
        }

        // 0 = justo en el umbral, 1 = vida casi a 0
        float intensity = 1f - (health01 / heartbeatStartThreshold);

        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer <= 0f)
        {
            float volume = Mathf.Lerp(minHeartbeatVolume, maxHeartbeatVolume, intensity);
            sfxSource.pitch = Random.Range(heartbeatPitchRange.x, heartbeatPitchRange.y);
            sfxSource.PlayOneShot(heartbeatClip, volume);

            heartbeatTimer = Mathf.Lerp(slowestInterval, fastestInterval, intensity);
        }
    }

    private void HandleDamage(float amount)
    {
        PlayRandom(damageClips, damageVolume, new Vector2(0.95f, 1.05f));
    }

    private void PlayRandom(AudioClip[] clips, float volume, Vector2 pitchRange)
    {
        if (clips == null || clips.Length == 0) return;
        sfxSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }
}