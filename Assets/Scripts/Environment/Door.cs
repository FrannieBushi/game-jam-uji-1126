using UnityEngine;
using System.Collections;
using UnityEditor.ShaderGraph;

public enum DoorType
{
    Normal, Knock, Locked
}

public enum DoorOrientation
{
    Left, Right, BackLeft, BackRight
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
    public virtual void OpenDoor()
    {
        PlayerDoorInteractor pdi = GameObject.FindWithTag("Player").GetComponent<PlayerDoorInteractor>();
        if(doorType==DoorType.Locked)
        {
            if(pdi.hasKey)
            {
                pdi.hasKey=false;
                StartCoroutine(OpenDoorRoutine());
            }
            else
            {
                Debug.Log("puerta bloqueada + no llave");
            }
        }
        else
            StartCoroutine(OpenDoorRoutine());
    }
        

    IEnumerator OpenDoorRoutine()
    {
        animator.Play("OpenDoor");

        if (blackOverlay != null) blackOverlay.FadeIn(1f);
        bool isBackDoor = doorOrientation == DoorOrientation.BackLeft || doorOrientation == DoorOrientation.BackRight;
        if (!isBackDoor) sideRoom.StoreReturnPosition();

        yield return new WaitForSeconds(1f);

        //llamar jumpscare aquí
        
        if (!isBackDoor)
        {
            sideRoom.DeleteSideRoom();
            if (doorOrientation == DoorOrientation.Left)
                sideRoom.GenerateSideRoom(doorType, DoorOrientation.BackRight);
            else if (doorOrientation == DoorOrientation.Right)
                sideRoom.GenerateSideRoom(doorType, DoorOrientation.BackLeft);

            sideRoom.TeleportPlayer(doorOrientation);
                    }
        else
        {
            sideRoom.TeleportPlayerBack();
        }

        yield return new WaitForSeconds(0.5f);
        if (blackOverlay != null) blackOverlay.FadeOut(1f);
    }
}
