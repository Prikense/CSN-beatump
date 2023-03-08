using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitboxStoreManager : MonoBehaviour
{

    //we store NAN hitboxes for now, for clarity i will be making one of this for every char, maybe for every enemy
    [SerializeField] public Hitbox[] hitboxListNAN = new Hitbox[]{
        //fuck me, hitbox initialization
        //2p -> 0
        new Hitbox ("NAN-2P", //anim name
        new Vector3 (1.55f, -0.745f, 0), //position
        new Vector3 (1.44f, 1f, 1f), //size
     //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?,  dmgZone
            true,  10,    70,      0,     45,       5,      12,       false,     false,   1),
        //5p -> 1
        new Hitbox ("NAN-5P", //anim name
        new Vector3 (1.8f, 0.2f, 0), //position
        new Vector3 (1.2f, 2.15f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  25,    150,      0,     30,       10,      30,        false,     false,  1),
        //2k -> 2
        new Hitbox ("NAN-2K", //anim name
        new Vector3 (1.1f, -1.1f, 0), //position
        new Vector3 (1.25f, 1.7f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  15,    85,      0,     -20,       5,      14,       false,     false,  2),
        //5k -> 3
        new Hitbox ("NAN-5K", //anim name
        new Vector3 (1.73f, -.5f, 0), //position
        new Vector3 (1.71f, 1.4f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?,  dmgZone
            false,  40,    180,      25,     25,       9,      25,       false,      true,   1),
        //2s -> 4
        new Hitbox ("NAN-2S", //anim name
        new Vector3 (2.25f, 0.16f, 0), //position
        new Vector3 (2f, 3.24f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  40,    90,      0,     25,       7,      16,       false,     false,  0),
        //5s -> 5
        new Hitbox ("NAN-5S", //anim name
        new Vector3 (3.275f, 0f, 0), //position
        new Vector3 (4f, .8f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  60,    90,      0,     10,       7,      17,       false,     false,  1),
        //2h -> 6
        new Hitbox ("NAN-2H", //anim name
        new Vector3 (1.48f, -.95f, 0), //position
        new Vector3 (1.85f, 1.8f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  30,    250,    80,     80,      6,      35,       false,     false,  1),
        //2h -> 7 2nd hit 
        new Hitbox ("NAN-2H", //anim name
        new Vector3 (2.25f, .83f, 0), //position
        new Vector3 (2f, 2.6f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  25,    300,      80,    90,       7,      35,       false,     false,  1),
        //5h -> 8
        new Hitbox ("NAN-5H", //anim name
        new Vector3 (1.4f, -.4f, 0), //position
        new Vector3 (2.9f, 1.15f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  75,    150,      0,     20,      10,      45,       true,     true,  1),
        //jp -> 9
        new Hitbox ("NAN-jP", //anim name
        new Vector3 (1.2f, -.63f, 0), //position
        new Vector3 (1f, 1.24f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
             true,  15,    20,      0,     5,      6,      15,       false,     false,  0),
        //jk -> 10
        new Hitbox ("NAN-jK", //anim name
        new Vector3 (1.95f, -.58f, 0), //position
        new Vector3 (1.15f, 2.46f, 1f), //size
      //gatCancel, dmg, knock, angleG, angleA, hitstop, hitstun, wallbounce?, kockdown?, dmgZone
            false,  25,    30,      0,     40,      7,      16,       false,     false,  0),
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
    
    //current active actors in the fight or smt
    [SerializeField] public EnemyJuggling enemy;
    [SerializeField] public movement playerMove;

    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        // hitboxInitializationNAN();
    }

    void FixedUpdate(){
        //works for now, dont forget to add the logic to get which player is currently active, also some grounded normals make you airborne so we add that exception
        if(playerMove.isGrounded != enemy.isGrounded ||
        ((!enemy.isGrounded && playerMove.animator.GetCurrentAnimatorStateInfo(0).IsTag("groundNormal")) ||
        (playerMove.jumpSquat && enemy.isGrounded))
        ){
            Physics2D.IgnoreCollision(playerMove.transform.GetComponent<Collider2D>(),enemy.transform.GetComponent<Collider2D>(), true);
        }else{
            Physics2D.IgnoreCollision(playerMove.transform.GetComponent<Collider2D>(),enemy.transform.GetComponent<Collider2D>(), false);
        }
    }

}
