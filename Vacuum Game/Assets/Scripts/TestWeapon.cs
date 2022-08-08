using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : IWeapon
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
}
