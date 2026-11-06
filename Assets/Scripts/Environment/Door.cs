using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    ScreenFader blackOverlay;

    void Awake()
    {
        blackOverlay = GameObject.FindWithTag("BlackOverlay").GetComponent<ScreenFader>();
    }
    public void OpenDoor()
    {
        StartCoroutine(OpenDoorRoutine());
    }

    public IEnumerator OpenDoorRoutine()
    {
        if(blackOverlay!=null) blackOverlay.FadeIn(1f);
        yield return new WaitForSeconds(1.5f);
        if(blackOverlay!=null) blackOverlay.FadeOut(1f);
    }
}
