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
    [SerializeField] private GameObject anchor;

    //input
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float keyX;
    [SerializeField] private float keyY;

    //animation stuff
    [SerializeField] private Animator animator;
    // [SerializeField] private bool jumpTime = false;//used for giving the initial jump boost
    [SerializeField] private bool jumpSquat =false;
    [SerializeField] private LayerMask gMask;
    [SerializeField] private InputsNAttacks inputScript;
    private bool specialBoost = false;
    // private float aux = 0;


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
        animator.SetInteger("SpeedY", (int)body.velocity.y);

        if(jumpSquat || isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")){
            Speed = transform.right* keyX;
            specialBoost = false;
        }
        //jumpsquat animation
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("groundNormal") || keyY < 0){//if you are attacking or crouching you cant move
            Speed = transform.right*0;
        }
       if(isGrounded && (keyY > 0 || jumpSquat) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("groundNormal")){
            jumpSquat = true;
            // body.velocity = new Vector2(body.velocity.x/3, 0);
        }

    //     //jump force
    //     if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jump") && jumpSquat){

    //         jumpTime = true;
    //         jumpSquat = false;
    //     }
        //crouch
        // if(isGrounded && keyY < 0){
           //here goes bool crouch = true;
           
        // }else{
            //make it false here
        // }
    }

    // Update is called once per frame
    void FixedUpdate(){
        MovementTime();
    }

    public void jumpTime(){
        jumpSquat=false;
        body.AddForce(Vector2.up*JumpHeight+jumpSpeed*Speed, ForceMode2D.Impulse);
    }

    void MovementTime(){
        if(!isGrounded){
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
