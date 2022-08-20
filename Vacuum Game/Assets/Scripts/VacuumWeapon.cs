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
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float slowSpeed;
    [SerializeField] float slowAccel;
    [SerializeField] float releaseAccel;
    [SerializeField] float currentSlowSpeed = 1f;
    [SerializeField] float damage;
    List<LivingEntity> suckedEnemies;
    // Start is called before the first frame update
    void Start()
    {
        suckedEnemies = new List<LivingEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFiring){
            float suckRadius = maxRadius;
            //RaycastHit2D hitLeft = Physics2D.Raycast(suckTransform.position, transform.up, maxRadius, blockLayers);
            RaycastHit2D hitMid = Physics2D.Raycast(suckTransform.position, transform.up, maxRadius, blockLayers);
            if(hitMid.collider != null){
                suckRadius = hitMid.distance;
            }
            Debug.Log(suckRadius);
            DirtController.instance.SuckSlice(suckTransform.position, transform.up * suckRadius, suckAngle, suckSpeed, particleKillRadius);
            SuckEnemies(suckRadius);
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

    void SuckEnemies(float radius){
        Collider2D[] cols = Physics2D.OverlapCircleAll(suckTransform.position, radius, enemyLayer);
        foreach(Collider2D col in cols){
            Vector2 bunnyDir = col.transform.position - suckTransform.position;
            //float angle = Mathf.Atan2(bunnyDir.y, bunnyDir.x) * Mathf.Rad2Deg - 90f;
            //angle = Mathf.Abs(angle);
            //float corrected = Mathf.Abs(angle - suckTransform.eulerAngles.z);
            //if(corrected > 180) corrected = 360 - corrected;
            //Debug.Log("Corrected: " + corrected);
            //Debug.Log("Angle: " + angle + ", Euler: " + suckTransform.eulerAngles.z);
            
            if((Vector2.Angle(bunnyDir, transform.up) < suckAngle / 2)){
                if(col.gameObject.tag == "Dust"){
                    LivingEntity living = col.gameObject.GetComponent<LivingEntity>();
                    living?.TakeDamage(damage * Time.deltaTime);
                    Debug.Log("Sucked BUNNY!");
                }
            }
            
            //if(Vector2.Angle((Vector2)suckTransform.position, (Vector2)col.transform.position) < suckAngle / 2f){
                
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
