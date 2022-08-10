using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumWeapon : MonoBehaviour, IWeapon
{
    bool isFiring = false;
    [SerializeField] float maxRadius;
    [SerializeField] float particleKillRadius;
    [SerializeField] float suckAngle;
    [SerializeField] float suckSpeed;
    [SerializeField] Transform suckTransform;
    [SerializeField] LayerMask blockLayers;
    [SerializeField] float slowSpeed;
    [SerializeField] float slowAccel;
    [SerializeField] float releaseAccel;
    [SerializeField] float currentSlowSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isFiring){
            float suckRadius = maxRadius;
            RaycastHit2D hit = Physics2D.Raycast(suckTransform.position, transform.up, maxRadius, blockLayers);
            if(hit.collider != null){
                suckRadius = hit.distance;
            }
            Debug.Log(suckRadius);
            DirtController.instance.SuckSlice(suckTransform.position, transform.up * suckRadius, suckAngle, suckSpeed, particleKillRadius);
            if(currentSlowSpeed > slowSpeed){
                currentSlowSpeed -= Time.deltaTime * slowAccel;
                currentSlowSpeed = Mathf.Max(currentSlowSpeed, slowSpeed);
            }
        }
        else{
            if(currentSlowSpeed < 1f){
                currentSlowSpeed += Time.deltaTime * releaseAccel;
                currentSlowSpeed = Mathf.Min(currentSlowSpeed, 1f);
            }
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
        //if(isFiring) return 0.3f;
        //return 1f;
        return currentSlowSpeed;
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
