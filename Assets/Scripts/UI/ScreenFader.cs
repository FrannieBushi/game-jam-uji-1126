using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    private CanvasGroup group;
    private Coroutine current;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    public void FadeIn(float duration)
    {
        StartFade(1f, duration);
    }

    public void FadeOut(float duration)
    {
        StartFade(0f, duration);
    }

    private void StartFade(float target, float duration)
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(Fade(target, duration));
    }

    private IEnumerator Fade(float target, float duration)
    {
        float start = group.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }

        group.alpha = target;
        current = null;
    }
}
