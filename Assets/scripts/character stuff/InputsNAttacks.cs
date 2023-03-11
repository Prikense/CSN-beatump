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
    // private int aux = 0; //this is used for more than 2 multihit attacks, to spawn the differente hitboxes past #2

    //hitbox stats, lets try using the class and not being idiots
    // [SerializeField] private Hitbox[] hitboxList;
    //we moved the hitbox store to another script so it isnt lost on load and easier managing
    [SerializeField] private hitboxStoreManager hitboxStorer;

    //attack stats
    [SerializeField] private int dmg = 0;
    [SerializeField] private int knockbackForce = 0;
    private bool wllBounce = false;
    private bool kockDown = false;
    private int dmgHLM = 0;
    private Vector3[] angle = {Vector3.zero, Vector3.zero}; // 0 -> ground, 1 -> air

    private int hitStun = 0;
    [SerializeField] public float hitStopTime = 0;
    [SerializeField] public bool hitStop = false;

    [SerializeField] private float bufferWindow = .05f;
    [SerializeField] private List<inputNtime> bufferlist = new List<inputNtime>();
    private EnemyJuggling EnScript;


    // Start is called before the first frame update
    void Awake()
    {
        // hitboxInitialization();        
        hitboxStorer = GameObject.FindObjectOfType<hitboxStoreManager>();

        body = transform.GetComponent<Rigidbody2D>();

        moveScript = transform.GetComponent<movement>();
 
        animator = transform.GetComponentInChildren<Animator>();

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


        //dont forget to add negative edge around here

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
        if(gatlingCancel && !hitStop && !moveScript.jumpSquat){
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
    }



    //for hitbox and gattling
    public void hitboxTrigger(Collider2D collider2Dhit){
        // Debug.Log("Hit was registered, dmg: "+dmg);
        gatlingCancel = true;
        //we deactivate the hitbox
        hitBoxstate = ColliderState.inactive;

        //enenmy dmg and hitstun time
        EnScript =collider2Dhit.GetComponent<EnemyJuggling>();
        EnScript.playerInScript = this;
        EnScript.health -= dmg;
        EnScript.hitStunAmount = hitStun;

        //we reset the velocity
        EnScript.body.velocity = Vector2.zero;
        //knockback
        if(EnScript.isGrounded){
            EnScript.body.velocity = knockbackForce*angle[0];
        }else{
            EnScript.body.velocity = knockbackForce*angle[1];
        }

        //for enemy dmg animation
        if(EnScript.isGrounded){
            switch(dmgHLM){
                case 0:
                    EnScript.animator.SetBool("hitF", true);
                    break;
                case 1:
                    EnScript.animator.SetBool("hitM", true);
                    break;
                case 2:
                    EnScript.animator.SetBool("hitL", true);
                    break;
            }
        }else{
            EnScript.animator.SetBool("hitM", true);
        }

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
    
    //spawn the hitbox with the ID from the hitbox array
    public void hitboxSpawn(int id){
        if((id == 17 || id == 11) && gatlingCancel){
            return;
        }
        //we create a new hitbox because fuck me idk
        Hitbox a = hitboxStorer.hitboxListNAN[id];

        //stats
        string name = a.stateName;
        boxPos = a.hitboxPos;
        boxSize = a.hitboxSize;
        gatlingCancel = a.selfCancel;
        dmg = a.Dmg;
        knockbackForce = a.knockbackForce;
        angle[0] =  a.GroundAngle;
        angle[1] = a.AirAngle;
        hitStopTime = a.charHitStop;
        hitStun = a.EnemyhitStun;
        wllBounce = a.doWallbounce;
        kockDown = a.knockdown;
        dmgHLM = a.dmgZone;

        hitBoxstate = ColliderState.active;

        switch (id){//for adding impulse on certain moves (it has to be done manually so yeah)
            case 0:
                break;
            case 3://5k
                body.velocity = 12*(transform.right)+8*transform.up;
                break;
            case 6://2h
                body.velocity = 10*(transform.right);
                break;
            default:
                break;
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
        //hitbox Special Cases DEACTIVATION, if in idle/walk or in air
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("idleNwalk") || 
           animator.GetCurrentAnimatorStateInfo(0).IsTag("air") ||
           animator.IsInTransition(0)){
            hitBoxstate = ColliderState.inactive;
            jumpCancel = false;
            // aux = 0;
        }

        //check for hitbox collision
        if(hitBoxstate == ColliderState.active){
            Collider2D collider2Dhit = Physics2D.OverlapBox(transform.position+boxPos, boxSize, 0f, layers);

            if(collider2Dhit != null){
                hitboxTrigger(collider2Dhit);
            }
        }
        //hitstop managing
        if(hitStop){
            if(animator.isActiveAndEnabled){
                // Debug.Log("sleepy time");
                animator.enabled = false;
                moveScript.Sleeptime();


                EnScript.Sleeptime();
            }
        }
        if(hitStopTime <= 0 && !animator.isActiveAndEnabled){    
            // Debug.Log("wakey wakey"); 
            animator.enabled = true;
            hitStop = false;
            moveScript.WakeTime();

            
            EnScript.animator.enabled = true;
            EnScript.WakeTime();
        }
    }
}