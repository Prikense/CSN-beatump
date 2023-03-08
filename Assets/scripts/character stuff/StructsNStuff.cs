using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Hitbox{

    public string stateName;
    public Vector3 hitboxPos;
    public Vector3 hitboxSize;
    public bool selfCancel;
    public int Dmg;
    public int knockbackForce;
    public Vector3 GroundAngle;
    public Vector3 AirAngle;
    public int charHitStop;
    public int EnemyhitStun;
    // public bool isLauncher;
    public bool doWallbounce;
    public bool knockdown;
    public int dmgZone; //value between 0 and 2 -> 0==high, 1==mid, 2==low

    public Hitbox(string name, Vector3 pos, Vector3 size, bool canCancel, int dmg, int knockback,
                  float groundAngle, float airAngle, int hitstop, int hitstun,// bool launch,
                  bool wallBounce, bool kockD, int a){
                    stateName = name;
                    hitboxPos = pos;
                    hitboxSize = size;
                    selfCancel = canCancel;
                    Dmg = dmg;
                    knockbackForce = knockback;
                    GroundAngle = new Vector3(Mathf.Cos(groundAngle*Mathf.PI/180f),Mathf.Sin(groundAngle*Mathf.PI/180f),0);
                    AirAngle = new Vector3(Mathf.Cos(airAngle*Mathf.PI/180f),Mathf.Sin(airAngle*Mathf.PI/180f),0);
                    charHitStop = hitstop;
                    EnemyhitStun = hitstun;
                    // isLauncher = launch;
                    doWallbounce = wallBounce;
                    knockdown = kockD;
                    dmgZone = a;

    }

}


public class inputNtime{
    public posibleInputs inputToDo;
    public float pressedTime;

    public inputNtime(posibleInputs a, float b){
        inputToDo = a;
        pressedTime = b;
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