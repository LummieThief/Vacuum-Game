using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : MonoBehaviour, IWeapon
{
    bool isFiring = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackDown(){
        isFiring = true;
        Debug.Log("Started Firing!");
    }

    public void AttackUp(){
        isFiring = false;
        Debug.Log("Stopped Firing!");
    }

    public void AltAttackDown(){
        isFiring = true;
        Debug.Log("Started Alt Firing!");
    }

    public void AltAttackUp(){
        isFiring = false;
        Debug.Log("Stopped Alt Firing!");
    }

    public float GetSlowPlayerMult(){
        return 1f;
    }

    public bool IsAttacking(){
        return isFiring;
    }
    public void Activate(){
        gameObject.SetActive(true);
    }
    public void Deactivate(){
        gameObject.SetActive(false);
    }
}
