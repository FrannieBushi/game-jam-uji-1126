using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    public static JumpscareManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Image jumpscareImage;          // full-screen Image on the Canvas
    [SerializeField] private Sprite[] jumpscareSprites;     // Rorschach pixel art

    [Header("Probability")]
    [Range(0f, 1f)] [SerializeField] private float baseChance = 0.9f; // chance when the curve returns 1
    [SerializeField] private float cooldown = 20f;                    // min seconds between jumpscares

    [Header("Timing")]
    [SerializeField] private float appearDelay = 1.2f;   // wait for the door fade-out to finish
    [SerializeField] private float displayTime = 0.6f;
    [SerializeField] private float shakeAmount = 8f;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpscareSound;

    private float lastJumpscareTime = -999f;
    private int lastSpriteIndex = -1;
    private bool isPlaying;

    void Awake()
    {
        Instance = this;
        if (jumpscareImage != null) jumpscareImage.enabled = false;
    }

    // Call this from Door when the open-door process ends
    public void TryJumpscare()
    {
        Debug.Log("entro jumpscare");
        if (isPlaying || jumpscareImage == null || jumpscareSprites.Length == 0) return;
        if (Time.time < lastJumpscareTime + cooldown) return;
        if (StressManager.Instance == null) return;

        float chance = Mathf.Clamp01(baseChance * StressManager.Instance.JumpscareChanceMultiplier);
        if (Random.value <= chance)
            StartCoroutine(JumpscareRoutine());
    }

    private IEnumerator JumpscareRoutine()
    {
        isPlaying = true;
        yield return new WaitForSeconds(appearDelay);

        jumpscareImage.sprite = PickSprite();
        jumpscareImage.enabled = true;

        if (audioSource != null && jumpscareSound != null)
            audioSource.PlayOneShot(jumpscareSound);

        RectTransform rect = jumpscareImage.rectTransform;
        Vector2 originalPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < displayTime)
        {
            rect.anchoredPosition = originalPos + Random.insideUnitCircle * shakeAmount;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = originalPos;
        jumpscareImage.enabled = false;

        lastJumpscareTime = Time.time;
        isPlaying = false;
    }

    // Random sprite, avoiding the same one twice in a row
    private Sprite PickSprite()
    {
        int index = Random.Range(0, jumpscareSprites.Length);
        if (jumpscareSprites.Length > 1)
            while (index == lastSpriteIndex)
                index = Random.Range(0, jumpscareSprites.Length);

        lastSpriteIndex = index;
        return jumpscareSprites[index];
    }
}
