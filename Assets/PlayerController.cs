using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject camHolder;
    public GameObject soundBroadcast;
    public int coinsCollected = 0;

    public float speed, sprintSpeed, slowSpeed, sensitivity, maxForce;
    private Vector2 move, look;
    private float lookRotation;

    private bool lockRotation = true;

    public AudioSource audioSrcSteps;
    public AudioSource audioSrcBreath;
    private float broadcastedSound;

    private bool isMoving = false;

    private float breath;

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
        Debug.Log("Breath: " + breath + " Breath sound: " + audioSrcBreath.volume);
        Move();
        ChangeSound();
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
            breath *= 1-0.01f*Time.deltaTime;
        }
        else if (isSlower)
        {
            targetVelocity *= slowSpeed;
            breath *= 1+0.01f*Time.deltaTime;
        }
        else // If the player is walking
        {
            targetVelocity *= speed;
            breath *= 1+0.005f*Time.deltaTime;
        }

        // Detects if the player is trying move
        if (targetVelocity.magnitude > 0.3f) {
            isMoving = true;
        }
        else {
            isMoving = false;
            breath *= 1+0.03f*Time.deltaTime;
        }
        
        breath = Mathf.Max(Mathf.Min(breath, 1),0); // Limits cumulative breath maximum to 1 (100%)

        targetVelocity *= breath; // Reduces the player's speed as it gets tired

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
        audioSrcSteps.volume = 0f;
        breath = 1; // Player starts with max breath
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Look();
    }

    void ChangeSound()
    {   
        ChangeSoundSteps();
        ChangeSoundBreath();

        //Calcula a distancia até onde será ouvido o som do player.
        broadcastedSound = Mathf.Sqrt(10000*(audioSrcSteps.volume+audioSrcBreath.volume)+1f);
        if (float.IsNaN(broadcastedSound))
        {
            broadcastedSound = 1f;
        }
        //Debug.Log(broadcastedSound);

        soundBroadcast.transform.localScale = new Vector3(broadcastedSound, 0.1f, broadcastedSound);
    }

    void ChangeSoundSteps() {
        // If the player is trying to move
        if(isMoving) {
            // Start the audio if it hasn't already
            if (!audioSrcSteps.isPlaying)
                audioSrcSteps.Play();

            // Changes the audio according to his speed
            audioSrcSteps.volume = Mathf.Pow((rb.velocity.magnitude-0.1f)/(sprintSpeed-0.1f), 1.46f);
            
            audioSrcSteps.pitch = rb.velocity.magnitude/3; // Adjusts pitch (audio playing speed) according to the velocity of the player
            if (audioSrcSteps.pitch > 1.8) // Limits maximum pitch
            {
                audioSrcSteps.pitch = 1.8f;
            }
            else if (audioSrcSteps.pitch < 0.5f) // Limits minimum pitch
            {
                audioSrcSteps.pitch = 0.5f;
            }
        }
        else if (audioSrcSteps.volume > 0.00001f) {
            audioSrcSteps.volume *= 0.85f;
        }
        else{
            audioSrcSteps.volume = 0f;
            audioSrcSteps.Stop();
        }
    }

    void ChangeSoundBreath() {
        audioSrcBreath.volume = 1-Mathf.Pow(breath,3);
    }

    // On collision with a monster, kills player
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Monster") {
            Destroy(gameObject);
        }
    }
}
