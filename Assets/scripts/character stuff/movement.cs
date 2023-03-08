using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    //movement stats
    [SerializeField]private int walkSpeed;
    [SerializeField]private int fallSpeed;
    [SerializeField]private int JumpHeight;
    [SerializeField]private int jumpSpeed;//initial left-right jump force
    [SerializeField]public bool isGrounded;
    [SerializeField] private Vector2 Speed;
    [SerializeField]private float currentFloorHeight;
    [SerializeField] public  GameObject anchor;

    //input
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float keyX;
    [SerializeField] private float keyY;

    //animation stuff
    [SerializeField] public Animator animator;
    // [SerializeField] private bool jumpTime = false;//used for giving the initial jump boost
    [SerializeField] public bool jumpSquat =false;
    [SerializeField] private LayerMask gMask;
    [SerializeField] private InputsNAttacks inputScript;
    private bool specialBoost = false;
    private float aux = 0;
    private float squatFrames = 3f/60f;
    [SerializeField] private bool dJump = true;
    private bool jumpHold = false;
    [SerializeField] private Vector2 velocitySave = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetComponent<Rigidbody2D>();

        inputScript = transform.GetComponent<InputsNAttacks>();
 
        animator = transform.GetComponentInChildren<Animator>();
    }

    //input managing
    void Update(){
        float keyY = Input.GetAxisRaw("Vertical");

        //walking
        //we get the input first
        float keyX = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(anchor.transform.position, 0.1f, gMask);

        animator.SetFloat("speed", Speed.x);
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("jumpAction", jumpSquat);
        animator.SetBool("jump2", false);
        animator.SetInteger("SpeedY", (int)body.velocity.y);

        //grounded variables
        Speed = transform.right* keyX;
        if(isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")){
            specialBoost = false;
            dJump = true;
        }
        //crouch stuff
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("groundNormal") || keyY < 0){//if you are attacking or crouching you cant move
            Speed = transform.right*0;
        }
        //jumpsquat animation
       if(isGrounded && (keyY > 0 || jumpSquat) && (animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk") || inputScript.jumpCancel)){
            jumpSquat = true;
            jumpHold = true;
            // body.velocity = new Vector2(body.velocity.x/3, 0);
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("groundNormal") && !inputScript.jumpCancel){
            jumpSquat =false;
        }
        if(jumpSquat){
            aux+=Time.deltaTime;
        }else{
            aux=0;
        }
        //up input manager
        if(keyY <= 0){
            jumpHold = false;
        }
        //double jump input
        if(!inputScript.hitStop && !jumpHold && !jumpSquat && dJump && !isGrounded && keyY > 0 && (animator.GetCurrentAnimatorStateInfo(0).IsTag("air") || inputScript.jumpCancel)){
            jumpTime2(keyX);
            animator.SetBool("jump2", true);
            dJump = false;
            // body.velocity = new Vector2(body.velocity.x/3, 0);
        }
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(!inputScript.hitStop){
            MovementTime();
        }
    }
    public void Sleeptime(){
        velocitySave = body.velocity;
        body.constraints = RigidbodyConstraints2D.FreezeAll | RigidbodyConstraints2D.FreezeRotation;
        //body.Sleep();
    }
    public void WakeTime(){
        body.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        body.velocity = velocitySave;
        //body.WakeUp();   
    }

    public void jumpTime(float a){
        aux=0;
        body.velocity = new Vector2 (body.velocity.x,0);
        body.AddForce(Vector2.up*JumpHeight+Vector2.right*jumpSpeed*a, ForceMode2D.Impulse);
    }
    public void jumpTime2(float a){
        aux=0;
        float expectedSpeed = (jumpSpeed*a);
        body.velocity = new Vector2 (body.velocity.x,0);
        //x speed capping
         if(Mathf.Abs(expectedSpeed - body.velocity.x) <= 3){
                body.velocity = new Vector2 (body.velocity.x,0);
            }else{
            body.velocity = new Vector2 (expectedSpeed,0);
        }
        body.AddForce(Vector2.up*JumpHeight/1.2f, ForceMode2D.Impulse);
    }


    void MovementTime(){
        if(aux>=squatFrames){
            jumpSquat=false;
            jumpTime(Input.GetAxisRaw("Horizontal"));
        }else if(!isGrounded){
            body.velocity -= new Vector2 (0,fallSpeed);
        }
        // if(jumpTime){
        //     body.AddForce(Vector2.up*JumpHeight+jumpSpeed*Speed, ForceMode2D.Impulse);
        //     jumpTime = jumpSquat;
        // }
        if(Speed.x != 0 && (isGrounded && !animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jump"))){
            body.velocity = Speed*walkSpeed*Time.deltaTime;
        }else if(isGrounded){
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5K")){
                //we reduce friction but also reset velocity yo 0 if they are going backwards
                body.velocity = new Vector2(Mathf.Max(0, body.velocity.x), body.velocity.y);
            }else if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5H") && !specialBoost){
                body.velocity = 8*(transform.right);
                specialBoost = true;//we only add this extra speed on the initial frames of this attack
            }else if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5S") && !specialBoost){
                body.velocity = 6*(transform.right);
                specialBoost = true;//we only add this extra speed on the initial frames of this attack
            }else{
                body.velocity -= body.velocity/6;
            }
        }
    }
}
