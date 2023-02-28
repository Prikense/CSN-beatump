using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsNAttacks : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float keyX;
    [SerializeField] private float keyY;

    //animation stuff
    [SerializeField] private Animator animator;
    [SerializeField] public movement moveScript;//to take isGrounded state


    //inputs for normal attacks
    [SerializeField] private bool punchButton = false;
    [SerializeField] private bool gatlingCancel = true;
    [SerializeField] private bool kickButton = false;
    [SerializeField] private bool slashButton = false;
    [SerializeField] private bool heavyButton = false;

    //hitbox Stuffs
    // [SerializeField] public GameObject hitbox; //lets try another aproach
    [SerializeField] public Vector3 boxSize = Vector3.one;
    [SerializeField] private Vector3 boxPos = Vector3.one;
    public Color inactiveColor;
    public Color activeColor;
    public  ColliderState hitBoxstate;

    [SerializeField] private LayerMask layers;


    //attack stats
    [SerializeField] private int dmg = 0;
    [SerializeField] private int knockbackForce = 0;

    [SerializeField] private float bufferWindow = .05f;
    [SerializeField] private List<inputNtime> bufferlist = new List<inputNtime>();


    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetComponent<Rigidbody2D>();

        moveScript = transform.GetComponent<movement>();
 
        animator = transform.GetComponentInChildren<Animator>();

        // hitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //we check if we are out of an attack animation and reset cancel bool
        if(!gatlingCancel && (animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk") || animator.GetCurrentAnimatorStateInfo(0).IsTag("air"))){
            gatlingCancel = true;
        }

        //we get the axis inputs for specials and some normals
        float keyY = Input.GetAxisRaw("Vertical");
        float keyX = Input.GetAxisRaw("Horizontal");

        //we reset the inputs
        punchButton = false;
        kickButton = false;
        slashButton = false;
        heavyButton = false;
        //we reset the inputs for the animator
        animator.SetBool("2p", punchButton);
        animator.SetBool("5p", punchButton);
        animator.SetBool("2k", kickButton);
        animator.SetBool("5k", kickButton);
        animator.SetBool("2s", kickButton);
        animator.SetBool("5s", slashButton);
        animator.SetBool("2h", heavyButton);
        animator.SetBool("5h", heavyButton);

        //when first pressed
        //old version
        // punchButton = Input.GetButtonDown("punch"); //punch       (a) (joy = 2)
        // kickButton  = Input.GetButtonDown("kick"); //kick        (b) (joy = 0)
        // slashButton = Input.GetButtonDown("slash"); //slash       (c) (joy = 3)
        // heavyButton = Input.GetButtonDown("heavy"); //heavy       (d) (joy = 1)
        //for releases, if the button was pressed this frame then we keep that input instead
        // punchButton = Input.GetButtonUp("punch") || punchButton;
        // kickButton  = Input.GetButtonUp("kick") || kickButton;
        // slashButton = Input.GetButtonUp("slash") || slashButton;
        // heavyButton = Input.GetButtonUp("heavy") || heavyButton;

        //we get the inputs
        if(Input.GetButtonDown("punch")){
            //we save the action and the time it was pressed
            bufferlist.Add(new inputNtime(posibleInputs.a, Time.time));
        }
        if(Input.GetButtonDown("kick")){
            //we save the action and the time it was pressed
            bufferlist.Add(new inputNtime(posibleInputs.b, Time.time));
        }
        if(Input.GetButtonDown("slash")){
            //we save the action and the time it was pressed
            bufferlist.Add(new inputNtime(posibleInputs.c, Time.time));
        }
        if(Input.GetButtonDown("heavy")){
            //we save the action and the time it was pressed
            bufferlist.Add(new inputNtime(posibleInputs.d, Time.time));
        }

        //buffer system
        if(gatlingCancel){
            if(bufferlist.Count > 0){
                foreach (inputNtime a in bufferlist.ToArray()){
                    bufferlist.Remove(a);
                    if(a.pressedTime+bufferWindow >= Time.time){
                        switch (a.inputToDo){
                            case posibleInputs.a:       //punch       (a) (joy = 2)
                                punchButton = true;
                                break;
                            case posibleInputs.b:       //kick        (b) (joy = 0)
                                kickButton = true;
                                break;
                            case posibleInputs.c:       //slash       (c) (joy = 3)
                                slashButton = true;
                                break;
                            case posibleInputs.d:       //heavy       (d) (joy = 1)
                                heavyButton = true;
                                break;
                        }
                    }
                }
            }
        }


       //2p
        if(gatlingCancel && punchButton && keyY < 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            // hitbox.SetActive(false);
            animator.SetBool("2p", punchButton);
            gatlingCancel = false;
        }
        //5p
        if(gatlingCancel && punchButton && keyY >= 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            animator.SetBool("5p", punchButton);
            gatlingCancel = false;
        }
        //2k
        if(gatlingCancel && kickButton && keyY < 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            animator.SetBool("2k", kickButton);
            gatlingCancel = false;
        }
        //5k
        if(gatlingCancel && kickButton && keyY >= 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2S") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5S") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5H") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            animator.SetBool("5k", kickButton);
            gatlingCancel = false;
        }
        //2s
        if(gatlingCancel && slashButton && keyY < 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5H") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            animator.SetBool("2s", slashButton);
            gatlingCancel = false;
        }
        //5s
        if(gatlingCancel && slashButton && keyY >= 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2S") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5H") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            animator.SetBool("5s", slashButton);
            gatlingCancel = false;
        }
        //2H
        if(gatlingCancel && heavyButton && keyY < 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2S") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5S") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            animator.SetBool("2h", heavyButton);
            gatlingCancel = false;
        }
       //5h
        if(gatlingCancel && heavyButton && keyY >= 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
            // hitbox.SetActive(false);
            animator.SetBool("5h", heavyButton);
            gatlingCancel = false;
        }

        //check for hitbox collision
        if(hitBoxstate == ColliderState.active){
            Collider2D collider2Dhit = Physics2D.OverlapBox(transform.position+boxPos, boxSize, 0f, layers);

            if(collider2Dhit != null){
                hitboxTrigger();
            }
        }
        // Debug.Log("hitbox status: "+hitBoxstate);
    }



    //for hitbox and gattling
    public void hitboxTrigger(){
        Debug.Log("Hit was registered, dmg: "+dmg);
        gatlingCancel = true;
        //we deactivate the hitbox
        // hitbox.SetActive(false);
        //knockback time
        body.AddForce(-knockbackForce*(transform.right), ForceMode2D.Impulse);
        hitBoxstate = ColliderState.inactive;
    }
    

    public void hitboxStuff(){
        //Debug.Log("hitbox stuff is active");
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P")){
            //hitbox activation
            boxPos = new Vector3 (1.55f, -0.745f, 0);
            boxSize = new Vector3 (1.44f, 1f, 1f);
            hitBoxstate = ColliderState.active;
            gatlingCancel = true;
            dmg = 10;
            knockbackForce = 3;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5P")){
            //hitbox activation
            boxPos = new Vector3 (1.8f, 0.2f, 0);
            boxSize = new Vector3 (1.2f, 2.15f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 30;
            knockbackForce = 8;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K")){
            //hitbox activation
            boxPos = new Vector3 (1.1f, -1.1f, 0);
            boxSize = new Vector3 (1.25f, 1.7f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 20;
            knockbackForce = 6;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5K")){
            //hitbox activation
            boxPos = new Vector3 (1.73f, -.5f, 0);
            boxSize = new Vector3 (1.71f, 1.4f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 60;
            knockbackForce = 6;
            body.velocity = 12*(transform.right)+8*transform.up;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2S")){
            //hitbox activation
            boxPos = new Vector3 (2.25f, 0.16f, 0);
            boxSize = new Vector3 (2f, 3.24f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 50;
            knockbackForce = 6;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5S")){
            //hitbox activation
            boxPos = new Vector3 (3.275f, 0f, 0);
            boxSize = new Vector3 (4f, 0.8f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 60;
            knockbackForce = 10;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2H")){
            //hitbox activation
            boxPos = new Vector3 (1.48f, -.95f, 0);
            boxSize = new Vector3 (1.85f, 1.8f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 30;
            knockbackForce = 3;
            body.velocity = 10*(transform.right);
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5H")){
            //hitbox activation
            boxPos = new Vector3 (1.4f, -.4f, 0);
            boxSize = new Vector3 (2.9f, 1.15f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 80;
            knockbackForce = 6;
        }
    }

    //for resizing or moving 2nd hitboxes on moves
    public void hitboxResizeNstuff(){
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2H")){
            //hitbox activation
            boxPos = new Vector3 (2.25f, .83f, 0);
            boxSize = new Vector3 (2f, 2.6f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 40;
            knockbackForce = 4;
        }
    }

    private void OnDrawGizmos() {
        changeHitboxColor();

        Gizmos.matrix = Matrix4x4.TRS(transform.position+boxPos, transform.rotation, transform.localScale);

        Gizmos.DrawCube(Vector3.zero, new Vector3(boxSize.x, boxSize.y, boxSize.z)); // Because size is halfExtents

    }

    private void changeHitboxColor() {
    switch(hitBoxstate) {
    case ColliderState.inactive:
        Gizmos.color = inactiveColor;
        break;
    case ColliderState.active:
        Gizmos.color = activeColor;
        break;
    }

}

    void FixedUpdate(){

    }
}

public enum ColliderState{
    active,
    inactive
}

public enum posibleInputs{
    a,
    b,
    c,
    d,
    e,
    assist,
    two,
    three,
    six,
    nine,
    eight,
    seven,
    four,
    one
}
public class inputNtime{
    public posibleInputs inputToDo;
    public float pressedTime;

    public inputNtime(posibleInputs a, float b){
        inputToDo = a;
        pressedTime = b;
    }
}