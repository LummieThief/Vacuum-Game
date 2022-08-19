using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(PlayerController player){
        PlayerController.instance.GrabFurniture(transform);
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player"){
            PlayerController.instance.interactable = this;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.tag == "Player"){
            if(PlayerController.instance.interactable is Furniture furn && furn == this){
                PlayerController.instance.interactable = null;
            }
        }
    }
}
