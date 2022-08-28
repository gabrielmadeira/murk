using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindMonsterStateManager : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource audioSrc;

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

    // Ground
    public GameObject planePrefab;

    // Ground Measurements
    [HideInInspector]
    public float scale_x;
    [HideInInspector]
    public float scale_z;

    public float minVol = 0.15f;

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
        audioSrc = GetComponent<AudioSource>();

        // Writes down size of the board
        measuresBoard();

        // Sets starting state for the state machine
        currentState = PatrolState;
        // 'this' is a reference to the current state  
        currentState.EnterState(this);
    }

    void FixedUpdate()
    {
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

    // On collision with a player, kills player
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Player") {
            Destroy(col.gameObject);
        }
    }


    public void SwitchState(BlindMonsterBaseState state) {
        // Transitions to the new state passed in
        currentState = state;
        // Calls EnterState logic from the new state one time
        state.EnterState(this);
    }


    public void measuresBoard() {
        // Writes down size of the board
        scale_x = 10*planePrefab.transform.localScale.x/2-2;
        if (scale_x < 0)
        {
            scale_x = 0;
        }
        scale_z = 10*planePrefab.transform.localScale.z/2-2;
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
}
