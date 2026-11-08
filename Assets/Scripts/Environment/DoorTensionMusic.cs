using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Reproduce música de tensión al cruzar una puerta.
/// No modifica Door.cs: detecta el uso de la puerta mirando Door.OnCooldown.
[RequireComponent(typeof(AudioSource))]
public class DoorTensionMusic : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Arrastra aquí las 5 canciones")]
    [SerializeField] private AudioClip[] tensionClips;
    [SerializeField, Range(0f, 1f)] private float volume = 0.7f;

    [Header("Tiempos")]
    [Tooltip("Espera tras abrir la puerta (1s = justo cuando la pantalla está en negro)")]
    [SerializeField] private float delayAfterDoor = 1f;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;

    [Header("Comportamiento")]
    [Tooltip("Al volver por una puerta Back, la música se detiene")]
    [SerializeField] private bool stopWhenGoingBack = true;
    [Tooltip("Solo suena para estos tipos de puerta")]
    [SerializeField] private bool playOnNormal = true;
    [SerializeField] private bool playOnKnock = true;
    [SerializeField] private bool playOnLocked = true;

    private AudioSource source;
    private Coroutine routine;
    private bool wasOnCooldown;

    // "Baraja" de canciones: se van sacando hasta vaciarla y luego se rebaraja
    private readonly List<AudioClip> bag = new List<AudioClip>();
    private AudioClip lastPlayed;

    /// Para cortar la musica desde otro sitio
    public void StopMusic() => StartFade(0f, fadeOutTime, true, 0f);

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D, suena igual en toda la escena
        source.loop = false;      // nunca en bucle
        source.volume = 0f;
    }

    void Update()
    {
        bool now = Door.OnCooldown;

        // Flanco de subida: una puerta acaba de empezar a abrirse
        if (now && !wasOnCooldown)
            OnDoorUsed();

        wasOnCooldown = now;
    }

    void OnDoorUsed()
    {
        Door used = FindOpeningDoor();

        bool isBack = used != null &&
            (used.doorOrientation == DoorOrientation.BackLeft ||
             used.doorOrientation == DoorOrientation.BackRight);

        if (isBack)
        {
            if (stopWhenGoingBack) StartFade(0f, fadeOutTime, stopAtEnd: true, delay: delayAfterDoor);
            return;
        }

        if (used != null && !AllowedType(used.doorType)) return;

        // Si ya está sonando una canción, la dejamos terminar
        if (source.isPlaying) return;

        StartFade(volume, fadeInTime, stopAtEnd: false, delay: delayAfterDoor, restart: true);
    }

    Door FindOpeningDoor()
    {
        foreach (Door d in FindObjectsByType<Door>(FindObjectsSortMode.None))
            if (d.IsOpening) return d;
        return null;
    }

    bool AllowedType(DoorType type)
    {
        switch (type)
        {
            case DoorType.Normal: return playOnNormal;
            case DoorType.Knock:  return playOnKnock;
            case DoorType.Locked: return playOnLocked;
            default: return true;
        }
    }

    // ---------- Selección aleatoria sin repetir hasta usar las 5 ----------

    AudioClip NextClip()
    {
        if (bag.Count == 0) RefillBag();
        if (bag.Count == 0) return null;

        AudioClip clip = bag[bag.Count - 1];
        bag.RemoveAt(bag.Count - 1);
        lastPlayed = clip;
        return clip;
    }

    void RefillBag()
    {
        bag.Clear();
        foreach (AudioClip c in tensionClips)
            if (c != null) bag.Add(c);

        // Fisher-Yates
        for (int i = bag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }

        // La siguiente en salir es la última de la lista: que no sea la que acaba de sonar
        if (bag.Count > 1 && bag[bag.Count - 1] == lastPlayed)
            (bag[0], bag[bag.Count - 1]) = (bag[bag.Count - 1], bag[0]);
    }

    // ---------- Fades ----------

    void StartFade(float target, float time, bool stopAtEnd, float delay, bool restart = false)
    {
        if (tensionClips == null || tensionClips.Length == 0) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeRoutine(target, time, stopAtEnd, delay, restart));
    }

    IEnumerator FadeRoutine(float target, float time, bool stopAtEnd, float delay, bool restart)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (restart && !source.isPlaying)
        {
            AudioClip clip = NextClip();
            if (clip == null) { routine = null; yield break; }

            source.clip = clip;
            source.volume = 0f;
            source.Play();
        }

        float start = source.volume;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }
        source.volume = target;

        if (stopAtEnd) source.Stop();
        routine = null;
    }

}
