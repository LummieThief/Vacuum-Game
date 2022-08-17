using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BroomWeapon : MonoBehaviour, IWeapon
{
    bool isFiring = false;
    //[SerializeField] float maxRadius;
    //[SerializeField] float particleKillRadius;
    //[SerializeField] float suckAngle;
    //[SerializeField] float suckSpeed;
    //[SerializeField] Transform suckTransform;
    //[SerializeField] LayerMask blockLayers;
    //[SerializeField] float slowSpeed;
    //[SerializeField] float slowAccel;
    //[SerializeField] float releaseAccel;
    bool isActivated = false;
    [SerializeField] float currentSlowSpeed = 1f;
    [SerializeField] float swingTime;
    [SerializeField] float swingAngle;
    private Vector3 leftRotation;
    private Vector3 rightRotation;
    private Tween tween;
    bool pointingRight = true;
    bool isSwinging = false;
    [SerializeField] Transform swingTransform;
    [SerializeField] Collider2D hitbox;
    [SerializeField] float damage;
    // Start is called before the first frame update
    void Awake()
    {
        isActivated = true;
        leftRotation = new Vector3(0, 0, swingAngle);
        rightRotation = new Vector3(0, 0, -swingAngle);
        swingTransform.eulerAngles = rightRotation;
        hitbox.enabled = false;
    }
    /*

    public void OnSpawn(WeaponsManager weaponsManager){
        if(!isActivated){
            isActivated = true;
            leftRotation = new Vector3(0, 0, swingAngle);
            rightRotation = new Vector3(0, 0, -swingAngle);
            swingTransform.eulerAngles = rightRotation;
            hitbox.enabled = false;
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        /*
        if(isFiring){
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
        */
        if(tween != null && !tween.active){
            EndAttack();
        }
    }

    void InitiateAttack(){
        if(pointingRight){
            tween = swingTransform.DOLocalRotate(leftRotation, swingTime);
        }
        else{
            tween = swingTransform.DOLocalRotate(rightRotation, swingTime);
        }
        pointingRight = !pointingRight;
        isSwinging = true;
        hitbox.enabled = true;
    }

    void EndAttack(){
        isSwinging = false;
        hitbox.enabled = false;
    }

    public void AttackDown(){
        isFiring = true;
        if(!isSwinging){
            InitiateAttack();
        }
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

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Rat"){
            LivingEntity livingEntity = other.gameObject.GetComponent<LivingEntity>();
            if(livingEntity != null){
                livingEntity.TakeDamage(damage);
            }
        }
    }
}
