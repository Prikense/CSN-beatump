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
    //hitbox stats, lets try using the class and not being idiots
    [SerializeField] private Hitbox[] hitboxList;

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
        hitboxInitialization();        

        body = transform.GetComponent<Rigidbody2D>();

        moveScript = transform.GetComponent<movement>();
 
        animator = transform.GetComponentInChildren<Animator>();

        // bodyCollider = transform.GetComponent<BoxCollider2D>();
    }

    void hitboxInitialization(){
        //fuck me, hitbox initialization
        hitboxList = new Hitbox[]{
        //2p -> 0
        new Hitbox ("NAN-2P", //anim name
        new Vector3 (1.55f, -0.745f, 0), //position
        new Vector3 (1.44f, 1f, 1f), //size
     //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?,  dmgZone
            true,  10,    40,      0,     30,       5,      12,       false,     false,   2),
        //5p -> 1
        new Hitbox ("NAN-5P", //anim name
        new Vector3 (1.8f, 0.2f, 0), //position
        new Vector3 (1.2f, 2.15f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  25,    38,      0,     5,       10,      26,        false,     false,  0),
        //2k -> 2
        new Hitbox ("NAN-2K", //anim name
        new Vector3 (1.1f, -1.1f, 0), //position
        new Vector3 (1.25f, 1.7f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  15,    26,      0,     30,       5,      12,       false,     false,  2),
        //5k -> 3
        new Hitbox ("NAN-5K", //anim name
        new Vector3 (1.73f, -.5f, 0), //position
        new Vector3 (1.71f, 1.4f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?,  dmgZone
            false,  40,    36,      0,     10,       9,      30,       false,      true,   1),
        //2s -> 4
        new Hitbox ("NAN-2S", //anim name
        new Vector3 (2.25f, 0.16f, 0), //position
        new Vector3 (2f, 3.24f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  40,    20,      0,     25,       7,      16,       false,     false,  0),
        //5s -> 5
        new Hitbox ("NAN-5S", //anim name
        new Vector3 (3.275f, 0f, 0), //position
        new Vector3 (4f, .8f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  60,    28,      0,     10,       7,      17,       false,     false,  1),
        //2h -> 6
        new Hitbox ("NAN-2H", //anim name
        new Vector3 (1.48f, -.95f, 0), //position
        new Vector3 (1.85f, 1.8f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  30,    40,      80,     80,      6,      45,       false,     false,  1),
        //2h -> 7 2nd hit 
        new Hitbox ("NAN-2H", //anim name
        new Vector3 (2.25f, .83f, 0), //position
        new Vector3 (2f, 2.6f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  25,    40,      80,   110,       7,      45,       false,     false,  1),
        //5h -> 8
        new Hitbox ("NAN-5H", //anim name
        new Vector3 (1.4f, -.4f, 0), //position
        new Vector3 (2.9f, 1.15f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  75,    45,      0,     10,      10,      45,       true,     true,  1),
        //jp -> 9
        new Hitbox ("NAN-jP", //anim name
        new Vector3 (1.2f, -.63f, 0), //position
        new Vector3 (1f, 1.24f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             true,  15,    15,      0,     5,      6,      15,       false,     false,  0),
        //jk -> 10
        new Hitbox ("NAN-jK", //anim name
        new Vector3 (1.95f, -.58f, 0), //position
        new Vector3 (1.15f, 2.46f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  25,    18,      0,     10,      7,      16,       false,     false,  0),
        //jk -> 11 late hitbox
        new Hitbox ("NAN-jK", //anim name
        new Vector3 (1.67f, -.17f, 0), //position
        new Vector3 (1.13f, .97f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  12,    9,       0,     10,       3,       7,       false,     false,  0),
        //js -> 12
        new Hitbox ("NAN-jS", //anim name
        new Vector3 (1.73f, -.71f, 0), //position
        new Vector3 (2.3f, 1.28f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             false,  25,    10,     0,      0,       6,       8,       false,     false,  0),
        //js -> 13 hitbox 2
        new Hitbox ("NAN-jS", //anim name
        new Vector3 (2.13f, -.55f, 0), //position
        new Vector3 (2.3f, 1.28f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             false,  15,    1,      0,      0,       4,       8,       false,     false,  0),
        //js -> 14 hitbox 3
        new Hitbox ("NAN-jS", //anim name
        new Vector3 (1.73f, -.71f, 0), //position
        new Vector3 (2.3f, 1.28f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             false,  15,    1,      0,      0,       4,       8,       false,     false,  0),
        //js -> 15 hitbox 4
        new Hitbox ("NAN-jS", //anim name
        new Vector3 (1.69f, -.35f, 0), //position
        new Vector3 (2.3f, 1.28f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             false,  25,    10,     0,     75,       7,      15,       false,     false,  0),
        //jh -> 16
        new Hitbox ("NAN-jH", //anim name
        new Vector3 (1.97f, .4f, 0), //position
        new Vector3 (1.8f, 1.47f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             false, 60,    25,     65,     65,       9,       30,       false,     true,  0),
        //jh -> 17 late hitbox
        new Hitbox ("NAN-jH", //anim name
        new Vector3 (1.57f, -.95f, 0), //position
        new Vector3 (1.94f, 2.17f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             false, 60,    25,     65,     65,       9,       30,       false,     true,  0),
        };
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


                EnScript.Sleeptime();
            }
        }
        if(hitStopTime <= 0 && !animator.isActiveAndEnabled){    
            Debug.Log("wakey wakey"); 
            animator.enabled = true;
            hitStop = false;
            moveScript.WakeTime();

            
            EnScript.animator.enabled = true;
            EnScript.WakeTime();
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

        //knockback
        if(EnScript.isGrounded){
            EnScript.body.AddForce(knockbackForce*angle[0], ForceMode2D.Impulse);
        }else{
            EnScript.body.AddForce(knockbackForce*angle[1], ForceMode2D.Impulse);
        }

        //for enemy dmg animation
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

        //old code
        // if(launcher){// for vertical launching moves
        //     EnScript.body.AddForce(knockbackForce*(transform.right+transform.up), ForceMode2D.Impulse);
        // }else if(EnScript.isGrounded){//if on the ground
        //     EnScript.body.AddForce(knockbackForce*(transform.right), ForceMode2D.Impulse);
        // }else{//a normal hit on air
        //     EnScript.body.AddForce(knockbackForce*(transform.right+transform.up/1.2f), ForceMode2D.Impulse);
        // }

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
        Hitbox a = hitboxList[id];

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
    }
}