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

    private float randomX;
    private float randomZ;

    private float patrolSpeed = 3;
    private float huntSpeed = 5;
    private float pace;

    // Place to walk to
    private Vector3 walkPoint;
    private Vector3 distanceToWalkPoint;
    private bool walkPointSet = false;

    // Last place player was seen at
    private Vector3 lastSeenAt;

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
            //Debug.Log("Ouvindo " + attentionSpam);
            pace = huntSpeed;
            audioSrc.volume = 1f;

            Hunt();
        }
        else if (attentionSpam > 0) { // Clueless State
            attentionSpam -= Time.deltaTime; // Counts down the monster's attention spam

            //Debug.Log("Prestando atenção " + attentionSpam);
            pace = patrolSpeed + (huntSpeed-patrolSpeed)*attentionSpam/15; // Gradually desacelerates the monster
            audioSrc.volume = 0.04f + 0.96f*attentionSpam/15; // Gradually makes the monster more silent
            Clueless();
        }
        else if (attentionSpam < 0) { // Stopped State
            attentionSpam += Time.deltaTime;

            //Debug.Log("Parado " + attentionSpam);
        }
        else { // Patrol State
            //Debug.Log("NAO Ouvindo " + attentionSpam);
            pace = patrolSpeed;
            audioSrc.volume = 0.04f;

            Patrol();
        }
        if (attentionSpam >= 0)
            Move();

        if (Mathf.Abs(attentionSpam) < 0.1f) {
            attentionSpam = 0f;
        }
    }

    // Moves the monster
    void Move() {
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
        walkPoint = lastSeenAt; //Just goes to where the player was last seen;
    }

    // Clueless
    private void Clueless() {
        if (!walkPointSet) {
            // Goes somewhere random from where you last were
            randomX = lastSeenAt.x + Random.Range(-15,15);
            randomZ = lastSeenAt.z + Random.Range(-15,15);
            SetWithingBounds();

            walkPoint = new Vector3(randomX, transform.position.y, randomZ);
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
            if (Random.Range(0,100) > 50)
            {
                attentionSpam = Random.Range(-10f,-5f);
            }
            else
            {
                SearchWalkPoint();
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
        randomX = transform.position.x + Random.Range(-40,40);
        randomZ = transform.position.z + Random.Range(-40,40);
        SetWithingBounds();

        walkPoint = new Vector3(randomX, transform.position.y, randomZ);
        walkPointSet = true;  
    }

    // Sets random walk to be within walls
    private void SetWithingBounds() {
        if (randomX > 0) {
            randomX = Mathf.Min(scale_x, randomX);
        }
        else {
            randomX = Mathf.Max(-scale_x, randomX);
        }

        if (randomZ > 0) {
            randomZ = Mathf.Min(scale_z, randomZ);
        }
        else {
            randomZ = Mathf.Max(-scale_z, randomZ);
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
            lastSeenAt = other.gameObject.transform.position;
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            isHearingPlayer = false;
        }
    }

    // On collision, kills player
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Player") {
            Destroy(col.gameObject);
        }
    }

}
