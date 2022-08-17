using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    int previousScene = 0;
    int currentScene = 0;
    int entrance = 0;
    List<RoomManager> rooms;
    Dictionary<int, bool> roomCleanedDict;

    void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }
        currentScene = SceneManager.GetActiveScene().buildIndex;
        roomCleanedDict = new Dictionary<int, bool>();
        roomCleanedDict.Add(currentScene, true);
    }

    void LoadScene(int newScene){
        previousScene = currentScene;
        currentScene = newScene;
        SceneManager.LoadScene(newScene);
        if(!roomCleanedDict.ContainsKey(currentScene)){
            roomCleanedDict.Add(currentScene, false);
        }
    }

    public void LoadRoom(int newScene, int newEntrance){
        if(newScene > 0 && newScene < SceneManager.sceneCount){
            entrance = newEntrance;
            LoadScene(newScene);
        }
        else{
            Debug.Log("Scene number " + newScene + " does not exist!");
        }
    }
    public int GetPreviousScene(){
        return previousScene;
    }
    public int GetEntrance(){
        return entrance;
    }

    public int GetCurrentScene(){
        return currentScene;
    }
    public void UpdateRoomClean(bool cleanState){
        if(!roomCleanedDict.ContainsKey(currentScene)){
            roomCleanedDict.Add(currentScene, cleanState);
        }
        else{
            roomCleanedDict[currentScene] = cleanState;
        }
    }

    public bool GetCurrentRoomCleaned(){
        if(roomCleanedDict.ContainsKey(currentScene)){
            return roomCleanedDict[currentScene];
        }
        return false;
    }
}
