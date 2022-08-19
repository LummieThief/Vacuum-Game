using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoseVacuumWeapon : MonoBehaviour, IWeapon
{
    bool isFiring = false;
    [SerializeField] float maxRadius;
    [SerializeField] float particleKillRadius;
    [SerializeField] float suckAngle;
    [SerializeField] float suckSpeed;
    [SerializeField] Transform suckTransform;
    [SerializeField] LayerMask blockLayers;
    [SerializeField] int maxCharge;
    int currentCharge = 0;
    void Update()
    {
        if(isFiring){
            float suckRadius = maxRadius;
            RaycastHit2D hit = Physics2D.Raycast(suckTransform.position, transform.up, maxRadius, blockLayers);
            if(hit.collider != null){
                suckRadius = hit.distance;
            }
            Debug.Log(suckRadius);
            currentCharge += DirtController.instance.SuckSlice(suckTransform.position, transform.up * suckRadius, suckAngle, suckSpeed, particleKillRadius);
            currentCharge = Mathf.Min(currentCharge, maxCharge);
        }
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
        isFiring = false;
        gameObject.SetActive(false);
    }
}
