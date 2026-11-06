using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    ScreenFader blackOverlay;
    Animator animator;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        blackOverlay = GameObject.FindWithTag("BlackOverlay").GetComponent<ScreenFader>();
    }
    public void OpenDoor()
    {
        StartCoroutine(OpenDoorRoutine());
    }

    public IEnumerator OpenDoorRoutine()
    {
        animator.Play("OpenDoor");
        if(blackOverlay!=null) blackOverlay.FadeIn(1f);
        yield return new WaitForSeconds(1.5f);
        if(blackOverlay!=null) blackOverlay.FadeOut(1f);
    }
}
