using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour, IInteractable
{
    public RoomManager roomManager;
    [SerializeField] private int sceneNum;
    [SerializeField] private int entranceNum;
    void Awake(){
        if(roomManager == null) roomManager = FindObjectOfType<RoomManager>();
    }
    public void Interact(PlayerController player){
        roomManager.ExitRoom(sceneNum, entranceNum);
    }
}
