using UnityEngine;
using System.Collections;
using UnityEditor.ShaderGraph;

public enum DoorType
{
    Normal, Knock, Locked
}

public enum DoorOrientation
{
    Left, Right, Back
}

public class Door : MonoBehaviour
{
    ScreenFader blackOverlay;
    Animator animator;
    SideRoomManager sideRoom;
    public DoorType doorType = DoorType.Normal;
    public DoorOrientation doorOrientation = DoorOrientation.Left;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        blackOverlay = GameObject.FindWithTag("BlackOverlay").GetComponent<ScreenFader>();
        sideRoom = GameObject.FindWithTag("GameController").GetComponent<SideRoomManager>();
        if(sideRoom==null) Debug.Log("FALTA UN GAMEOBJECT VACIO CON TAG SIDEROOM!!!!!");
    }
    public void OpenDoor()
    {
        StartCoroutine(OpenDoorRoutine());
    }

    IEnumerator OpenDoorRoutine()
    {
        animator.Play("OpenDoor");
        if(blackOverlay!=null) blackOverlay.FadeIn(1f);
        yield return new WaitForSeconds(1f);

        if(doorOrientation!=DoorOrientation.Back)
        {
            sideRoom.DeleteSideRoom();
            sideRoom.GenerateSideRoom(doorType);
            sideRoom.TeleportPlayer(doorOrientation);
        }
        else
        {
            
        }

        yield return new WaitForSeconds(0.5f);
        if(blackOverlay!=null) blackOverlay.FadeOut(1f);
    }
}
