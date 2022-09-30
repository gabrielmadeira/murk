using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindMonsterStateManager : MonoBehaviour
{
    public Rigidbody rb;
    
    // Audio Sources
    public AudioSource stepsAudioSrc;
    public AudioSource growlAudioSrc;
    public List<AudioClip> growls;

    private int chosenGrowl;
    private int lastChosenGrowl = 0;

    // Uncertainty zone
    public GameObject uncertaintyZonePrefab;
    [HideInInspector]
    public GameObject uncertaintyZone;
    [HideInInspector]
    public Vector3 playerLastHeardAt;
    [HideInInspector]
    public float certainty;
    [HideInInspector]
    public float uncertaintyZoneDiameter;

    // Ground Measurements
    [HideInInspector]
    public float scale_x;
    [HideInInspector]
    public float scale_z;

    public float minStepVol;

    public float patrolSpeed = 3;
    public float huntSpeed = 5;
    [HideInInspector]
    public float pace;

    BlindMonsterBaseState currentState;
    public BlindMonsterCluelessState CluelessState = new BlindMonsterCluelessState();
    public BlindMonsterHuntState HuntState = new BlindMonsterHuntState();
    public BlindMonsterPatrolState PatrolState = new BlindMonsterPatrolState();
    public BlindMonsterStopState StopState = new BlindMonsterStopState();

    void Start()
    {
        // Gets audio source
        //audioSrc = GetComponent<AudioSource>();

        // Writes down size of the board
        measuresBoard();

        // Sets starting state for the state machine
        currentState = PatrolState;
        // 'this' is a reference to the current state  
        currentState.EnterState(this);
    }

    void FixedUpdate()
    {
        // Updates the monster's stepping audio  
        float currentMonsterVel = Vector3.Magnitude(rb.velocity);
        stepsAudioSrc.volume = currentMonsterVel/huntSpeed; // Changes the monster's step volume based on his speed
        if (currentMonsterVel>patrolSpeed)
            stepsAudioSrc.pitch = currentMonsterVel/patrolSpeed; // Changes the monsters pitch (velocity of his steps) if he is trying to go slightly faster
        else
            stepsAudioSrc.pitch = 1;
        //Debug.Log("Vol: " + stepsAudioSrc.volume + " Pitch: " + stepsAudioSrc.pitch);

        currentState.FixedUpdateState(this);
    }

    void OnTriggerEnter(Collider other) {
        currentState.OnTriggerEnterState(this, other);
    }

    void OnTriggerStay(Collider other) {
        currentState.OnTriggerStayState(this, other);
    }

    void OnTriggerExit(Collider other) {
        currentState.OnTriggerExitState(this, other);
    }

    public void SwitchState(BlindMonsterBaseState state) {
        // Transitions to the new state passed in
        currentState = state;
        // Calls EnterState logic from the new state one time
        state.EnterState(this);
    }


    public void measuresBoard() {
        // Writes down size of the board
        scale_x = 10*OptionsMenu.mapSizeX/2-2;
        if (scale_x < 0)
        {
            scale_x = 0;
        }
        scale_z = 10*OptionsMenu.mapSizeZ/2-2;
        if (scale_z < 0)
        {
            scale_z = 0;
        }
    }

    public Vector3 SetWithingBounds(Vector3 walkPoint) {
        // Sets random destination to be within walls
        if (walkPoint.x > 0) {
            walkPoint.x = Mathf.Min(scale_x, walkPoint.x);
        }
        else {
            walkPoint.x = Mathf.Max(-scale_x, walkPoint.x);
        }

        if (walkPoint.z > 0) {
            walkPoint.z = Mathf.Min(scale_z, walkPoint.z);
        }
        else {
            walkPoint.z = Mathf.Max(-scale_z, walkPoint.z);
        }

        return walkPoint;
    }

    public void PlayGrowl() {
        // Plays and aggresive growl  
        if (!growlAudioSrc.isPlaying) // If no audio clip is being played
        {
            do { // Selects a new growl audio clip
                chosenGrowl = Random.Range(1,growls.Count);
            } while (lastChosenGrowl == chosenGrowl);
            lastChosenGrowl = chosenGrowl;

            growlAudioSrc.clip = growls[chosenGrowl]; 
            growlAudioSrc.Play(); // Plays it
        }
    }
}
