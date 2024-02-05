using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{

    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 10f;
    private float speed;
    private Rigidbody2D rb;
    private Vector2 stickInput;

    // Start is called before the first frame update
    void Start()
    {
        speed = walkingSpeed;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        float xStick = Input.GetAxis("Horizontal");
        float yStick = Input.GetAxis("Vertical");
        stickInput = new Vector2(xStick,yStick);

        if(Input.GetButtonDown("punch")){
            speed = runningSpeed;
        }
        if(Input.GetButtonUp("punch")){
            speed = walkingSpeed;
        }
    }

    private void FixedUpdate() {
        //Physics
        rb.MovePosition(rb.position + stickInput * speed * Time.fixedDeltaTime);
    }
}
