using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    bool isLoading = false;
    bool roomClean = false;
    Entrance[] entrances;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        entrances = FindObjectsOfType<Entrance>();
        if (LevelLoader.instance != null && LevelLoader.instance.GetCurrentRoomCleaned()){
            roomClean = true;
            // tell dirt controller that the room is clean
        }
        else{
            if(DirtController.instance != null && !DirtController.instance.hasSpawnedStartParticles){
                DirtController.instance.AddStartingParticles();
            }
            
            // tell dirt controller to do normal stuff
        }
        Vector2 spawnPos = transform.position;
        if(entrances.Length > 0) spawnPos = entrances[0].spawn.position;
        int prevScene = 0;
        int entrNum = 0;
        if(LevelLoader.instance != null){
            prevScene = LevelLoader.instance.GetPreviousScene();
            entrNum = LevelLoader.instance.GetEntrance();
        }
        
        foreach(Entrance entr in entrances){
            if(entr.sceneNum == prevScene && entr.entranceNum == entrNum){
                spawnPos = entr.spawn.position;
                break;
            }
        }
        player?.SpawnPlayerAt(spawnPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitRoom(int newScene, int newEntrance){
        if(!isLoading){
            PlayerController.instance?.StopPlayer();
            LevelLoader.instance?.LoadRoom(newScene, newEntrance);
            isLoading = true;
        }
    }
}
