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
    //List<RoomManager> rooms;
    Dictionary<int, bool> roomCleanedDict;

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
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
            if (currentScene == 1)
			{
                roomCleanedDict[currentScene] = true;
			}
        }

        MusicController.instance.SwapTrack(roomCleanedDict[currentScene]);
    }

    public void LoadRoom(int newScene, int newEntrance){
        if(newScene >= 0 && newScene < SceneManager.sceneCountInBuildSettings){
            entrance = newEntrance;
            LoadScene(newScene);
            Popup.instance?.SetText("");
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
        MusicController.instance.SwapTrack(cleanState);
    }

    public bool GetCurrentRoomCleaned(){
        if(roomCleanedDict.ContainsKey(currentScene)){
            return roomCleanedDict[currentScene];
        }
        return false;
    }
}
