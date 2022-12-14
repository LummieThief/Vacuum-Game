using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour, IInteractable
{
    public RoomManager roomManager;
    public Transform spawn;
    public int sceneNum;
    public int entranceNum;
    [SerializeField] string message;
    void Awake(){
        if(roomManager == null) roomManager = FindObjectOfType<RoomManager>();
        if(spawn == null) spawn = transform;
    }
    public void Interact(PlayerController player){
        roomManager.ExitRoom(sceneNum, entranceNum);
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player"){
            PlayerController.instance.interactable = this;
            Popup.instance?.SetText(message);
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.tag == "Player"){
            Popup.instance?.SetText("");
            if (PlayerController.instance.interactable is Entrance entr && entr == this){
                PlayerController.instance.interactable = null;
            }
        }
    }
}
