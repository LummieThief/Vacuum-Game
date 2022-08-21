using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingEntity
{
    public static PlayerController instance;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float acceleration;
    [SerializeField] Camera cam;
    //[SerializeField] Transform crosshair;
    private Rigidbody2D rb;
    private WeaponsManager wm;
    private enum PlayerState{
        Idle, Dead, Hold, Stop
    };
    private PlayerState currentState = PlayerState.Idle;
    //Vector2 moveDir;
    Vector2 currentVelocity;
    //Vector2 mousePos;
    public FrameInput input;
    public IInteractable interactable {get; set;}
    IInteractable holdInteract;
    private Transform holdItem;
    private Animator _anim;
    private string currentAnim = "PlayerIdle";

    void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }
        base.Awake();
        currentState = PlayerState.Idle;
        rb = GetComponent<Rigidbody2D>();
        wm = GetComponent<WeaponsManager>();
        _anim = GetComponentInChildren<Animator>();
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
        wm.ReceiveInput(input, currentState == PlayerState.Idle);
        //Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 desiredVelocity = input.movement.normalized * moveSpeed * wm.GetWeaponSpeedMult();
        //currentVelocity = 
        //mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        //if(crosshair != null) crosshair.position = input.mousePos;
        
        Vector2 lookDir = input.mousePos - (Vector2)transform.position;
        if(input.interactDown && (interactable != null || holdInteract != null)) Interact();
    }

    void GatherInput(){
        input = new FrameInput{
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition),
            attackDown = Input.GetButtonDown("Fire1"),
            attackUp = Input.GetButtonUp("Fire1"),
            altAttackDown = Input.GetButtonDown("Fire2"),
            altAttackUp = Input.GetButtonUp("Fire2"),
            interactDown = Input.GetButtonDown("Interact")
        };
    }

    void FixedUpdate()
    {
        if(currentState != PlayerState.Dead && currentState != PlayerState.Stop){
            Move();
            if(currentState != PlayerState.Hold) Aim(input.mousePos);
        }
        if(interactable != null) Debug.Log("Touching Interactable!");
        UpdateAnimation(GetAnimState());
    }

    void Move(){
        Vector2 desiredVelocity = input.movement.normalized * moveSpeed * wm.GetWeaponSpeedMult();
        Vector2 diffVector = desiredVelocity - currentVelocity;
        float frameAccel = Mathf.Clamp(acceleration * Time.deltaTime, 0, 1);
        Vector2 changeVector = diffVector * frameAccel;
        currentVelocity = currentVelocity + changeVector;

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void Aim(Vector2 aimPos){
        //Vector2 aimPos = input.mousePos;
        if(currentState == PlayerState.Hold && holdItem != null){
            //aimPos = holdItem.position;
        }
        Vector2 lookDir = aimPos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.eulerAngles = new Vector3(0, 0, angle);
        //weaponsRotator.eulerAngles = new Vector3(0, 0, angle);
    }

    private void Interact(){
        if(holdInteract != null) ReleaseFurniture();
        else interactable?.Interact(this);
    }

    public void GrabFurniture(Transform furn){
        holdItem = furn;
        holdInteract = interactable;
        Aim(holdItem.position);
        furn.SetParent(transform);
        currentState = PlayerState.Hold;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void ReleaseFurniture(){
        if(holdItem != null){
            holdItem.SetParent(null);
            holdItem = null;
            interactable = holdInteract;
            holdInteract = null;
            if(currentState == PlayerState.Hold){
                currentState = PlayerState.Idle;
            }
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }

    public void StopPlayer(){
        if(currentState != PlayerState.Dead) currentState = PlayerState.Stop;
    }

    public void SpawnPlayerAt(Vector2 spawnPos){
        currentState = PlayerState.Idle;
        transform.position = spawnPos;
    }

    string GetAnimState(){
        if(currentState != PlayerState.Idle && currentState != PlayerState.Hold) return "PlayerIdle";
        if(currentVelocity.magnitude < 0.3f) return currentState == PlayerState.Idle ? "PlayerIdle" : "PlayerAttackIdle";
        else return currentState == PlayerState.Idle ? "PlayerWalk" : "PlayerAttackWalk";
        //return "PlayerIdle";
    }

    void UpdateAnimation(string newAnim){
        if(!newAnim.Equals(currentAnim)){
            currentAnim = newAnim;
            _anim.CrossFade(currentAnim, 0, 0);
        }
    }


    public struct FrameInput {
        public Vector2 movement;
        public Vector2 mousePos;
        public bool attackDown;
        public bool attackUp;
        public bool altAttackDown;
        public bool altAttackUp;
        public bool interactDown;
    }
}
