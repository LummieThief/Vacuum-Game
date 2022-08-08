using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    [SerializeField] IWeapon currentWeapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReceiveInput(bool attackDown, bool attackUp){
        if(attackDown) currentWeapon.AttackDown();
        else if(attackUp) currentWeapon.AttackUp();
    }
}
