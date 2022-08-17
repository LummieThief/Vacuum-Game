using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingEntity
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float acceleration;
    [SerializeField] Camera cam;
    [SerializeField] Transform crosshair;
    private Rigidbody2D rb;
    private WeaponsManager wm;
    //Vector2 moveDir;
    Vector2 currentVelocity;
    //Vector2 mousePos;
    public FrameInput input;
    public IInteractable interactable {get; set;}

    void Awake(){
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        wm = GetComponent<WeaponsManager>();
        input = new FrameInput{
            movement = Vector2.zero,
            mousePos = Vector2.zero,
            attackDown = false,
            attackUp = false,
            altAttackDown = false,
            altAttackUp = false
        };
    }

    void Update()
    {
        GatherInput();
        wm.ReceiveInput(input);
        //Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 desiredVelocity = input.movement.normalized * moveSpeed * wm.GetWeaponSpeedMult();
        //currentVelocity = 
        //mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        if(crosshair != null) crosshair.position = input.mousePos;
        
        Vector2 lookDir = input.mousePos - (Vector2)transform.position;
        if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Pressed Damage");
            TakeDamage(0f);
        }
    }

    void GatherInput(){
        input = new FrameInput{
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition),
            attackDown = Input.GetButtonDown("Fire1"),
            attackUp = Input.GetButtonUp("Fire1"),
            altAttackDown = Input.GetButtonDown("Fire2"),
            altAttackUp = Input.GetButtonUp("Fire2")
        };
    }

    void FixedUpdate()
    {
        Vector2 desiredVelocity = input.movement.normalized * moveSpeed * wm.GetWeaponSpeedMult();
        Vector2 diffVector = desiredVelocity - currentVelocity;
        float frameAccel = Mathf.Clamp(acceleration * Time.deltaTime, 0, 1);
        Vector2 changeVector = diffVector * frameAccel;
        currentVelocity = currentVelocity + changeVector;

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
        Vector2 lookDir = input.mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.eulerAngles = new Vector3(0, 0, angle);
        //weaponsRotator.eulerAngles = new Vector3(0, 0, angle);
    }

    private void Interact(){
        interactable?.Interact(this);
    }

    public struct FrameInput {
        public Vector2 movement;
        public Vector2 mousePos;
        public bool attackDown;
        public bool attackUp;
        public bool altAttackDown;
        public bool altAttackUp;
    }
}
