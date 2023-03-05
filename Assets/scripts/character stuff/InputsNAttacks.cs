using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsNAttacks : MonoBehaviour
{
    //rigidboy and collider
    [SerializeField] private Rigidbody2D body;
    // private BoxCollider2D bodyCollider;

    //directional inputs
    [SerializeField] private float keyX;
    [SerializeField] private float keyY;

    //animation stuff
    [SerializeField] private Animator animator;
    [SerializeField] public movement moveScript;//to take isGrounded state


    //inputs for normal attacks
    [SerializeField] private bool punchButton = false;
    [SerializeField] private bool gatlingCancel = true;
    [SerializeField] public bool jumpCancel = false;
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
    private int aux = 0; //this is used for more than 2 multihit attacks, to spawn the differente hitboxes past #2


    //attack stats
    [SerializeField] private int dmg = 0;
    [SerializeField] private int knockbackForce = 0;
    [SerializeField] public float hitStopTime = 0;
    [SerializeField] public bool hitStop = false;

    [SerializeField] private float bufferWindow = .05f;
    [SerializeField] private List<inputNtime> bufferlist = new List<inputNtime>();


    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetComponent<Rigidbody2D>();

        moveScript = transform.GetComponent<movement>();
 
        animator = transform.GetComponentInChildren<Animator>();

        // bodyCollider = transform.GetComponent<BoxCollider2D>();
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
        //air bools
        animator.SetBool("jp", punchButton);
        animator.SetBool("jk", kickButton);
        animator.SetBool("js", slashButton);
        animator.SetBool("jh", heavyButton);


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
        if(gatlingCancel && !hitStop){
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

        //Ground normals - STARTUP
       //2p
        if(gatlingCancel && punchButton && keyY < 0 && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2P") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk")
        )){
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
            animator.SetBool("5h", heavyButton);
            gatlingCancel = false;
        }


        //Air normals - STARTUP
       //jp
        if(gatlingCancel && punchButton && !moveScript.isGrounded && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jP") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jK") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jS") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jH") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("air"))
        ){
            animator.SetBool("jp", punchButton);
            gatlingCancel = false;
        }
        //jK
        if(gatlingCancel && kickButton && !moveScript.isGrounded && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jP") || 
        animator.GetCurrentAnimatorStateInfo(0).IsTag("air"))
        ){
            animator.SetBool("jk", kickButton);
            gatlingCancel = false;
        }
       //jS
        if(gatlingCancel && slashButton && !moveScript.isGrounded && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jP") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jK") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("air"))
        ){
            animator.SetBool("js", slashButton);
            gatlingCancel = false;
        }
        //jH
        if(gatlingCancel && heavyButton && !moveScript.isGrounded && //gatling table
        (animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jP") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jK") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jS") ||
        animator.GetCurrentAnimatorStateInfo(0).IsTag("air"))
        ){
            animator.SetBool("jh", heavyButton);
            gatlingCancel = false;
        }

        //check for hitbox collision
        if(hitBoxstate == ColliderState.active){
            Collider2D collider2Dhit = Physics2D.OverlapBox(transform.position+boxPos, boxSize, 0f, layers);

            if(collider2Dhit != null){
                hitboxTrigger(collider2Dhit);
            }
        }

        //hitbox Special Cases DEACTIVATION, if in idle/walk or in air
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk") || 
           animator.GetCurrentAnimatorStateInfo(0).IsTag("air") ||
           animator.IsInTransition(0)){
            hitBoxstate = ColliderState.inactive;
            jumpCancel = false;
            aux = 0;
           }
        

        //hitstop managing
        if(hitStop){
            if(animator.isActiveAndEnabled){
                Debug.Log("sleepy time");
                animator.enabled = false;
                moveScript.Sleeptime();
            }
        }
        if(hitStopTime <= 0 && !animator.isActiveAndEnabled){    
            Debug.Log("wakey wakey");        
            animator.enabled = true;
            hitStop = false;
            moveScript.WakeTime();
        }

        // Debug.Log("animation time: "+animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }



    //for hitbox and gattling
    public void hitboxTrigger(Collider2D collider2Dhit){
        // Debug.Log("Hit was registered, dmg: "+dmg);
        gatlingCancel = true;
        //we deactivate the hitbox
        hitBoxstate = ColliderState.inactive;
        //enenmy knockback time
        // collider2Dhit.GetComponent<Rigidbody2D>().AddForce(knockbackForce*(transform.right), ForceMode2D.Impulse);
        
        //jump cancel for 2k, 2s and 2h
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K") ||
           animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2S") || 
           animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2H") ||
           animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jP") ||
           animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jK") ||
           animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jH")){
            jumpCancel = true;
        }
        hitStop = true;
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
            hitStopTime = 5;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5P")){
            //hitbox activation
            boxPos = new Vector3 (1.8f, 0.2f, 0);
            boxSize = new Vector3 (1.2f, 2.15f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 30;
            knockbackForce = 18;
            hitStopTime = 8;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2K")){
            //hitbox activation
            boxPos = new Vector3 (1.1f, -1.1f, 0);
            boxSize = new Vector3 (1.25f, 1.7f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 20;
            knockbackForce = 16;
            hitStopTime = 5;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5K")){
            //hitbox activation
            boxPos = new Vector3 (1.73f, -.5f, 0);
            boxSize = new Vector3 (1.71f, 1.4f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 60;
            knockbackForce = 20;
            hitStopTime = 9;
            body.velocity = 12*(transform.right)+8*transform.up;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2S")){
            //hitbox activation
            boxPos = new Vector3 (2.25f, 0.16f, 0);
            boxSize = new Vector3 (2f, 3.24f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 50;
            knockbackForce = 15;
            hitStopTime = 5;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5S")){
            //hitbox activation
            boxPos = new Vector3 (3.275f, 0f, 0);
            boxSize = new Vector3 (4f, 0.8f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 60;
            knockbackForce = 18;
            hitStopTime = 6;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2H")){
            //hitbox activation
            boxPos = new Vector3 (1.48f, -.95f, 0);
            boxSize = new Vector3 (1.85f, 1.8f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 30;
            knockbackForce = 5;
            hitStopTime = 6;
            body.velocity = 10*(transform.right);
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-5H")){
            //hitbox activation
            boxPos = new Vector3 (1.4f, -.4f, 0);
            boxSize = new Vector3 (2.9f, 1.15f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 80;
            knockbackForce = 22;
            hitStopTime = 10;
        }

        //air normals
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jP")){
            //hitbox activation
            boxPos = new Vector3 (1.2f, -.63f, 0);
            boxSize = new Vector3 (1f, 1.24f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 15;
            knockbackForce = 3;
            hitStopTime = 5;
            gatlingCancel = true;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jK")){
            //hitbox activation
            boxPos = new Vector3 (1.95f, -.58f, 0);
            boxSize = new Vector3 (1.15f, 2.46f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 25;
            knockbackForce = 12;
            hitStopTime = 6;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jS")){
            //hitbox activation
            boxPos = new Vector3 (1.73f, -.71f, 0);
            boxSize = new Vector3 (2.3f, 1.28f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 30;
            if(aux != 0){
                dmg = 20;
            }
            knockbackForce = 4;
            hitStopTime = 4;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jH")){
            //hitbox activation
            boxPos = new Vector3 (1.97f, .4f, 0);
            boxSize = new Vector3 (1.8f, 1.47f, 1f);
            hitBoxstate = ColliderState.active;
            dmg = 70;
            knockbackForce = 25;
            hitStopTime = 9;
        }
    }

    //for resizing or moving 2nd hitboxes on moves
    public void hitboxResizeNstuff(){
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .14f){
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-2H")){
                //hitbox activation
                boxPos = new Vector3 (2.25f, .83f, 0);
                boxSize = new Vector3 (2f, 2.6f, 1f);
                hitBoxstate = ColliderState.active;
                dmg = 40;
                knockbackForce = 12;
                hitStopTime = 8;
            }
            //for air hitboxes
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jK") && !gatlingCancel){
                //hitbox activation
                boxPos = new Vector3 (1.67f, -.17f, 0);
                boxSize = new Vector3 (1.13f, .97f, 1f);
                hitBoxstate = ColliderState.active;
                dmg = 12;
                knockbackForce = 9;
                hitStopTime = 3;
            }
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jS")){
                switch(aux){
                    case 0:
                        //hitbox activation
                        boxPos = new Vector3 (2.13f, -.55f, 0);
                        boxSize = new Vector3 (2.3f, 1.28f, 1f);
                        hitBoxstate = ColliderState.active;
                        dmg = 20;
                        aux=1;
                        knockbackForce = 15;
                        hitStopTime = 4;
                        break;
                    case 1:
                        //hitbox activation
                        boxPos = new Vector3 (1.69f, -.35f, 0);
                        boxSize = new Vector3 (1.52f, 1.8f, 1f);
                        hitBoxstate = ColliderState.active;
                        dmg = 40;
                        aux =0;
                        knockbackForce = 15;
                        hitStopTime = 6;
                        break;
                }
            }
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("NAN-jH") && !gatlingCancel){
                //hitbox activation
                boxPos = new Vector3 (1.57f, -.95f, 0);
                boxSize = new Vector3 (1.94f, 2.17f, 1f);
                hitBoxstate = ColliderState.active;
                dmg = 70;
                knockbackForce = 25;
                hitStopTime = 9;
            }
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
        if(hitStop){
            hitStopTime -= 1;
        }
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