using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindMonster : MonoBehaviour
{
    public Rigidbody rb;
    AudioSource audioSrc;

    // Ground
    public GameObject planePrefab;

    // Ground Measurements
    private float scale_x;
    private float scale_z;

    private Vector2 randomXZ;

    private float patrolSpeed = 3;
    private float huntSpeed = 5;
    private float pace;

    // Place to walk to
    private Vector3 walkPoint;
    private Vector3 distanceToWalkPoint;
    private bool walkPointSet = false;

    // Last place player was seen at
    private Vector3 lastHeardAt;

    private bool isHearingPlayer = false;
    private float attentionSpam = 0f; 

    // Start is called before the first frame update
    void Start()
    {
        // Gets audio source
        audioSrc = GetComponent<AudioSource>();

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

    // Update is called once per frame
    void FixedUpdate()
    {
        // Poorly made state machine:
        if (isHearingPlayer) // Hunt State
        {
            pace = huntSpeed;
            audioSrc.volume = 1f;

            Hunt();

            Debug.Log("Ouvindo " + attentionSpam);
        }
        else if (attentionSpam >= 0.1f) { // Clueless State
            attentionSpam -= Time.deltaTime; // Counts down the monster's attention spam

            pace = patrolSpeed + (huntSpeed-patrolSpeed)*attentionSpam/15; // Gradually desacelerates the monster
            audioSrc.volume = 0.04f + 0.96f*attentionSpam/15; // Gradually makes the monster more silent
            
            Clueless();

            Debug.Log("Prestando atenção " + attentionSpam);
        }
        else if (attentionSpam <= 0) { // Stopped State
            attentionSpam += Time.deltaTime;

            Debug.Log("Parado " + attentionSpam);
        }
        else { // Patrol State
            pace = patrolSpeed;
            audioSrc.volume = 0.04f;

            Patrol();

            Debug.Log("NAO Ouvindo " + attentionSpam);
        }
        if (attentionSpam >= 0)
            Move();
    }

    void Move() {
        // Moves the monster
        //Debug.Log("Walk Point = " + walkPoint);
        distanceToWalkPoint = transform.position-walkPoint;

        //find target velocity
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(-distanceToWalkPoint.x,0,-distanceToWalkPoint.z);

        targetVelocity = targetVelocity.normalized*pace;

        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange.y = 0;

        //Limit force
        Vector3.ClampMagnitude(velocityChange, 1f);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    // Hunts
    private void Hunt() {
        walkPoint = lastHeardAt; //Just goes to where the player was last seen;
    }

    // Clueless
    private void Clueless() {
        if (!walkPointSet) {
            // Goes somewhere random from where you last were
            randomXZ = Random.insideUnitCircle * 17;
            randomXZ[0] += lastHeardAt.x;
            randomXZ[1] += lastHeardAt.z;

            SetWithingBounds();

            walkPoint = new Vector3(randomXZ[0], transform.position.y, randomXZ[1]);
            walkPointSet = true;
        }
        else {
            distanceToWalkPoint = transform.position-walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }
    }

    // Patrols
    private void Patrol() {
        if (!walkPointSet) {
            if (Random.Range(0,100) > 50) // Has a 50% chance of stopping
            {
                attentionSpam = Random.Range(-10f,-5f); // Stops for 5 to 10 seconds
            }
            else
            {
                SearchWalkPoint(); // Finds a new walkpoint
            }
        }
        else {
            distanceToWalkPoint = transform.position-walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }
    }

    // Creates a new point to walk to 
    private void SearchWalkPoint() {
        randomXZ = Random.insideUnitCircle * 45;
        randomXZ[0] += transform.position.x;
        randomXZ[1] += transform.position.z;
        
        SetWithingBounds();

        walkPoint = new Vector3(randomXZ[0], transform.position.y, randomXZ[1]);
        walkPointSet = true;  
    }

    // Sets random walk to be within walls
    private void SetWithingBounds() {
        if (randomXZ[0] > 0) {
            randomXZ[0] = Mathf.Min(scale_x, randomXZ[0]);
        }
        else {
            randomXZ[0] = Mathf.Max(-scale_x, randomXZ[0]);
        }

        if (randomXZ[1] > 0) {
            randomXZ[1] = Mathf.Min(scale_z, randomXZ[1]);
        }
        else {
            randomXZ[1] = Mathf.Max(-scale_z, randomXZ[1]);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            isHearingPlayer = true;

            // Preparations for Clueless State
            walkPointSet = false;
            attentionSpam = Random.Range(15f,25f);
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            lastHeardAt = other.gameObject.transform.position;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            isHearingPlayer = false;
        }
    }

    // On collision with a player, kills player
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Player") {
            Destroy(col.gameObject);
        }
    }

}
