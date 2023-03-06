using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJuggling : MonoBehaviour
{
    //hitstun
    [SerializeField] public int hitStunAmount = 0;
    //Stats
    [SerializeField] public int health = 1000;
    [SerializeField] private int fallSpeed = 1;
    [SerializeField] public Rigidbody2D body;
    [SerializeField]public bool isGrounded;
    [SerializeField] public  GameObject anchor;
    [SerializeField] private LayerMask gMask;
    [SerializeField] private Vector2 velocitySave = Vector2.zero;
    //animator stuff
    [SerializeField] public Animator animator;
    [SerializeField] public InputsNAttacks playerInScript;

    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetComponent<Rigidbody2D>();

        animator = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        isGrounded = Physics2D.OverlapCircle(anchor.transform.position, 0.1f, gMask);
        //animator variables update
        animator.SetBool("grounded", isGrounded);
        animator.SetInteger("SpeedY", (int)body.velocity.y);
    }

    void FixedUpdate(){
        //gravity
        if(!isGrounded){
            body.velocity -= new Vector2 (0,fallSpeed);
        }
        if(hitStunAmount > 0 && !playerInScript.hitStop){
            hitStunAmount--;
        }
    }

    public void Sleeptime(){
        velocitySave = body.velocity;
        body.constraints = RigidbodyConstraints2D.FreezeAll | RigidbodyConstraints2D.FreezeRotation;
    }
    public void WakeTime(){
        body.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        body.velocity = velocitySave;
    }
}
