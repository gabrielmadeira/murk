using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject camHolder;
    public float speed, sprintSpeed, slowSpeed, sensitivity, maxForce;
    private Vector2 move, look;
    private float lookRotation;
    private bool lockRotation = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
       bool isSlower = Input.GetKey(KeyCode.LeftShift);
       bool isSprinting = Input.GetKey(KeyCode.Space);

        //find target velocity
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(move.x,0,move.y);
        if (isSprinting)
        {
            targetVelocity *= sprintSpeed;
        }
        else if (isSlower)
        {
            targetVelocity *= slowSpeed;
        }
        else
        {
            targetVelocity *= speed;
        }

        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange.y = 0;

        //Limit force
        Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Look()
    {
        //Turn
        transform.Rotate(Vector3.up * look.x * sensitivity);

        if (!lockRotation)
        {
            //look
            lookRotation += (-look.y * sensitivity);
        }
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Look();
    }
}
