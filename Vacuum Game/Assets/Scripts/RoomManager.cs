using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    bool isLoading = false;
    bool roomClean = false;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (LevelLoader.instance.GetCurrentRoomCleaned()){
            roomClean = true;
            // tell dirt controller that the room is clean
        }
        else{
            // tell dirt controller to do normal stuff
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitRoom(int newScene, int newEntrance){
        if(!isLoading){
            LevelLoader.instance.LoadRoom(newScene, newEntrance);
            isLoading = true;
        }
    }
}
