using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    IWeapon currentWeapon;
    List<IWeapon> weapons;
    [SerializeField] GameObject[] weaponPrefabs;
    int weaponIndex = 0;
    [SerializeField] Transform weaponHold;
    bool canAttack = true;
    // Start is called before the first frame update
    void Awake()
    {
        weapons = new List<IWeapon>();
        foreach(GameObject gunPrefab in weaponPrefabs){
            GameObject gun = Instantiate(gunPrefab, weaponHold.position, weaponHold.rotation);
            gun.transform.parent = weaponHold;
            IWeapon newWeapon = gun.GetComponent<IWeapon>();
            if(newWeapon != null) {
                weapons.Add(newWeapon);
                if(currentWeapon == null) currentWeapon = newWeapon;
                else newWeapon.Deactivate();
            }
        }
        //if(weapons != null && weapons.Count > 0) currentWeapon = weapons[weaponIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveInput(PlayerController.FrameInput newInput, bool newCanAttack){ //bool attackDown, bool attackUp, bool altAttackDown, bool altAttackUp
        if(canAttack && !newCanAttack) currentWeapon.Deactivate();
        else if(!canAttack && newCanAttack) currentWeapon.Activate();
        canAttack = newCanAttack;
        if(canAttack){
            if(newInput.attackDown) currentWeapon.AttackDown();
            else if(newInput.attackUp) currentWeapon.AttackUp();
            if(newInput.altAttackDown) currentWeapon.AltAttackDown();
            else if(newInput.altAttackUp) currentWeapon.AltAttackUp();
        }
        
    }
    public float GetWeaponSpeedMult(){
        return currentWeapon.GetSlowPlayerMult();
    }

    void ChangeWeaponIndex(float change){
        if(change == 0) return;
        else if(change < 0){
            weaponIndex--;
            if(weaponIndex < 0) weaponIndex = weapons.Count - 1;
        }
        else if(change > 0){
            weaponIndex++;
            if(weaponIndex >= weapons.Count) weaponIndex = 0;
        }
        SetNewWeapon();
    }

    void SetNewWeapon(){
        currentWeapon.Deactivate();
        currentWeapon = weapons[weaponIndex];
        currentWeapon.Activate();
    }
}
