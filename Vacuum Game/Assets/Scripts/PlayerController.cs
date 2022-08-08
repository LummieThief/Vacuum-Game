using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingEntity
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Camera cam;
    [SerializeField] Transform crosshair;
    private Rigidbody2D rb;
    private WeaponsManager wm;
    Vector2 moveDir;
    Vector2 currentVelocity;
    Vector2 mousePos;

    void Awake(){
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        wm = GetComponent<WeaponsManager>();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        currentVelocity = moveInput.normalized * moveSpeed;
        mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        if(crosshair != null) crosshair.position = mousePos;
        
        Vector2 lookDir = mousePos - (Vector2)transform.position;
        if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Pressed Damage");
            TakeDamage(0f);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.eulerAngles = new Vector3(0, 0, angle);
        //weaponsRotator.eulerAngles = new Vector3(0, 0, angle);
    }
}
