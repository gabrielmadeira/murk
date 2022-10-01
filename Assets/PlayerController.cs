using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject camHolder;
    public GameObject soundBroadcast;

    public float speed, sprintSpeed, slowSpeed, sensitivity, maxForce;
    private Vector2 move, look;
    private float lookRotation;

    private bool lockRotation = true;

    // - SOUND -------------- 
    public GameObject ObjectMusic; // Universal narrator
    private AudioSource voiceAudioSrc; // Narrator audio source
    private float maximumLoudness = 0.5f;
    private float pitchMultiplier = 0.8f;

    public AudioClip introAudio;
    public AudioClip gameplayInstructionsAudio;
    public AudioClip youDiedAudio;
    public AudioClip youLeftAudio;

    private bool playedInstructionAudio = false;

    public AudioSource audioSrcSteps;
    public AudioSource audioSrcBreath;
    private float breath;

    private float stepingLoudness;
    private float breathLoudness;
    private float audioReach;

    public float updateStep = 0.05f;
    public int sampleDataLength = 1024;
 
    private float currentUpdateTime = 0f;
 
    private float clipLoudness;
    private float[] clipSampleData;
    //------------------------

    private bool isMoving = false;

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
        //Debug.Log("Breath: " + breath + " Breath sound: " + audioSrcBreath.volume);
        //Debug.Log(darkCamera.enabled);
        Move();
        ChangeSound();

        if (playedInstructionAudio == false && !voiceAudioSrc.isPlaying){
            PlayAudioClip(gameplayInstructionsAudio);
            playedInstructionAudio = true;
        }
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
            breath *= 1-0.008f*Time.deltaTime;
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
        ObjectMusic = GameObject.FindWithTag("Narrator"); // Gets the universal narrator
        voiceAudioSrc = ObjectMusic.GetComponent<AudioSource>();
        
        Cursor.lockState = CursorLockMode.Locked;
        audioSrcSteps.volume = 0f;
        breath = 1; // Player starts with max breath

        clipSampleData = new float[sampleDataLength];

        PlayAudioClip(introAudio); // Passa instruções para o player
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LateUpdate()
    {
        if (!PauseMenu.gameIsPaused)
            Look();
    }

    void ChangeSound()
    {   
        ChangeSoundSteps();
        ChangeSoundBreath();

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep) {
            currentUpdateTime = 0f;

            // Calcula a loudness dos passos do player
            stepingLoudness = ClipLoudnessCalculator(audioSrcSteps);
            if (float.IsNaN(stepingLoudness))
                stepingLoudness = 0f;
            
            // Calcula a loudness da respiração do player
            breathLoudness = ClipLoudnessCalculator(audioSrcBreath);
            if (float.IsNaN(breathLoudness))
                breathLoudness = 0f;

            //Calcula a distancia até onde será ouvido o som do player.
            audioReach = Mathf.Sqrt(500000*(stepingLoudness+breathLoudness)/maximumLoudness+1f);

            soundBroadcast.transform.localScale = new Vector3(audioReach, 0.1f, audioReach);   
        }
    }

    float ClipLoudnessCalculator(AudioSource audioSrc) {
        audioSrc.clip.GetData(clipSampleData, audioSrc.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
        clipLoudness = 0f;
        foreach (var sample in clipSampleData) {
            clipLoudness += Mathf.Abs(sample);
        }
        return clipLoudness * audioSrc.volume / sampleDataLength;
    }

    void ChangeSoundSteps() {
        // If the player is trying to move
        if(isMoving) {
            // Start the audio if it hasn't already
            if (!audioSrcSteps.isPlaying)
                audioSrcSteps.Play();

            // Changes the audio according to his speed
            audioSrcSteps.volume = Mathf.Pow((rb.velocity.magnitude-0.1f)/(sprintSpeed-0.1f), 1.46f)*maximumLoudness;
            
            audioSrcSteps.pitch = rb.velocity.magnitude/3; // Adjusts pitch (audio playing speed) according to the velocity of the player
            if (audioSrcSteps.pitch > 1.8) // Limits maximum pitch
            {
                audioSrcSteps.pitch = 1.8f;
            }
            else if (audioSrcSteps.pitch < 0.5f) // Limits minimum pitch
            {
                audioSrcSteps.pitch = 0.5f;
            }
            audioSrcSteps.pitch *= pitchMultiplier; 
        }
        else if (audioSrcSteps.volume > 0.00001f) { // Slowly fades away the walking sound
            audioSrcSteps.volume *= 0.85f;
        }
        else{
            audioSrcSteps.volume = 0f;
            audioSrcSteps.Stop();
        }
    }

    void ChangeSoundBreath() {
        audioSrcBreath.volume = (1-Mathf.Pow(breath,3)); // The more out of breath you are, the louder you breath
        audioSrcBreath.volume *= (0.25f + 0.75f*Vector3.Magnitude(rb.velocity/sprintSpeed)); // The faster you are going, the louder you breath
        //audioSrcBreath.volume *= maximumLoudness; // Restricts player sound to the maximum (from 0 to 1)
    }

    // On collision with a monster, ends the game
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Monster") {
            PlayAudioClip(youDiedAudio);
            EndGame();
        }
    }

    void EndGame() { // Ends game by going back to menu
        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.LoadScene(3);
    }

    void PlayAudioClip(AudioClip soundClip)
    {
        voiceAudioSrc.clip = soundClip;
        voiceAudioSrc.Play();
    }
}
