using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationEventsHandler : MonoBehaviour
{

    //this script redirects animation events to the inputsNAttacks script
    [SerializeField] private InputsNAttacks inputScript;


    // Start is called before the first frame update
    void Start()
    {
        inputScript = transform.GetComponentInParent<InputsNAttacks>(true);
    }
    // public void jumpEvent(){
    //     inputScript.moveScript.jumpTime();
    // }
    // public void hitboxActive(){
    //     inputScript.hitboxStuff();
    // }
    // public void hitboxChange(){
    //     inputScript.hitboxResizeNstuff();
    // }
    // public void hitboxActive(){
    //     inputScript.hitboxStuff();
    // }
    public void hitboxFunc(int a){
        inputScript.hitboxSpawn(a);
    }

    public void hitboxDeactivate(){
        // inputScript.hitbox.SetActive(false);
        inputScript.hitBoxstate = ColliderState.inactive;
    }
}
