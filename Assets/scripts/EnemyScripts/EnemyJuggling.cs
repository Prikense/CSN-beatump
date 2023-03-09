using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJuggling : MonoBehaviour
{
    //hitstun
    [SerializeField] public int hitStunAmount = 0;
    private int totalHitStun = 14;
    //animation takes 14 frames
    //if hitstun is greater we should play the animation a little slower
    //(i think it should be 20 tho maybe because we eat the first frame that is 5 or 6 frames less?)

    //Stats
    [SerializeField] public int health = 1000;
    [SerializeField] private int fallSpeed = 1;
    [SerializeField] public Rigidbody2D body;
    [SerializeField]public bool isGrounded;
    [SerializeField] public  GameObject anchor;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Vector2 velocitySave = Vector2.zero;
    //animator stuff
    [SerializeField] public Animator animator;
    [SerializeField] public InputsNAttacks playerInScript;

    [SerializeField] private Transform sprite;
    private int aux = 2;

    private hitboxStoreManager hitboxStorer;

    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetComponent<Rigidbody2D>();

        animator = transform.GetComponentInChildren<Animator>();

        sprite = transform.GetChild(0);

        hitboxStorer = GameObject.FindObjectOfType<hitboxStoreManager>();
    }


    void FixedUpdate(){
        //ground check
        if(transform.position.y + body.velocity.y*Time.fixedDeltaTime <= hitboxStorer.groundLevel){
            transform.position = new Vector3(transform.position.x,hitboxStorer.groundLevel,transform.position.z);
            body.velocity = new Vector2 (body.velocity.x, 0);
        }
        if(transform.position.y <= hitboxStorer.groundLevel){
            isGrounded = true;
            // transform.position = new Vector3(transform.position.x,hitboxStorer.groundLevel,transform.position.z);
        }else{
            isGrounded = false;
        }

        //animator variables update
        animator.SetBool("grounded", isGrounded);
        animator.SetInteger("SpeedY", (int)body.velocity.y);
        animator.SetInteger("hitStun", hitStunAmount);
        animator.SetFloat("animSpeed", 14f/totalHitStun);

        if (aux == 1){
            aux--;
        }else if(aux == 0){
            animator.enabled = false;
            aux = 2;
        }
        frictionAndGrav();

        if(hitStunAmount > 0 && !playerInScript.hitStop){
            hitStunAmount--;
        }
        if(hitStunAmount == 0){
            totalHitStun = 14;
        }


        try{
            if(playerInScript.hitStop){
                if(Time.frameCount % 2 == 0){
                    sprite.position += Vector3.right*(-.1f);
                }else{
                    sprite.position += Vector3.right*(.1f);
                }
            }

        }catch{
            return;
        }
    }

    public void Sleeptime(){
        velocitySave = body.velocity;
        totalHitStun = hitStunAmount;
        body.constraints = RigidbodyConstraints2D.FreezeAll | RigidbodyConstraints2D.FreezeRotation;
        // animator.SetFloat("animSpeed", 20f/totalHitStun);
        aux = 1;
    }

    public void WakeTime(){
        body.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        body.velocity = velocitySave;

        animator.SetBool("hitF", false);
        animator.SetBool("hitM", false);
        animator.SetBool("hitL", false);
    }

    void frictionAndGrav(){
        //gravity
        if(!isGrounded){
            body.velocity -= new Vector2 (0,fallSpeed);
        }else{//friction
        // body.velocity = new Vector2 (body.velocity.x, 0);
            body.velocity -= body.velocity/6;
        }
    }
}
