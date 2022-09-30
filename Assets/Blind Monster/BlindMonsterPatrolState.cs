using UnityEngine;

public class BlindMonsterPatrolState : BlindMonsterBaseState
{
    private GameObject Goal;

    private Vector3 walkPoint;
    private bool walkPointSet;

    private float distanceToWalkPoint;
    private float oldDistanceToWalkPoint;

    private float walkAttemptDuration;

    public override void EnterState(BlindMonsterStateManager monster) {
        //Debug.Log("Patrol");
        Goal = GameObject.FindWithTag("Goal"); // Follows the goal sound

        monster.pace = monster.patrolSpeed;
        
        walkPointSet = false;
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {

        if (!monster.growlAudioSrc.isPlaying) // Just spams the monster's growl if nothing else is being played
        {
            monster.growlAudioSrc.clip = monster.growls[0]; // Sets the monster's growl back to its default
            monster.growlAudioSrc.volume = 0.3f;
            monster.growlAudioSrc.Play();
        }

        if (!walkPointSet)
        {
            SearchWalkPoint(monster); // Finds a new walkpoint
            oldDistanceToWalkPoint = Vector3.Distance(monster.transform.position, walkPoint);

            walkAttemptDuration = Random.Range(3f,5f); // The monster will check every 4s to 6s if it managed to move to its destination;
        }
        else {
            distanceToWalkPoint = Vector3.Distance(monster.transform.position, walkPoint);
            
            walkAttemptDuration -= Time.deltaTime;
            if (walkAttemptDuration < 0)
            {
                if (distanceToWalkPoint+1 >= oldDistanceToWalkPoint) // Checks if the monster is making no progress towards its destination
                    walkPointSet = false;
                    //Debug.Log(monster.name + "IS STUCK");

                oldDistanceToWalkPoint = distanceToWalkPoint; // Saves distance to checkpoint at the start of the new attemp to walk

                walkAttemptDuration = Random.Range(3f,5f); // The monster will check every 4s to 6s if it managed to move to its destination;
            }
                
            //Walkpoint reached
            if (distanceToWalkPoint < 1f)
            {
                walkPointSet = false;
                
                if (Random.Range(0,100) > 50f) // Has some chance of stopping instead of walking
                    monster.SwitchState(monster.StopState);
            }   
        }
        Move(monster, walkPoint);
    }

    public override void OnTriggerEnterState(BlindMonsterStateManager monster, Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            monster.playerLastHeardAt = other.gameObject.transform.position;
            monster.SwitchState(monster.HuntState);
        }
    }

    public override void OnTriggerStayState(BlindMonsterStateManager monster, Collider other) {

    }

    public override void OnTriggerExitState(BlindMonsterStateManager monster, Collider other) {

    }

    private void SearchWalkPoint(BlindMonsterStateManager monster) {
        // Creates a new point to walk to it
        walkPoint = monster.transform.position;

        Vector2 randomXZ = Random.insideUnitCircle * 45;
        walkPoint.x += randomXZ[0];
        walkPoint.z += randomXZ[1];
        
        // Adds preference to move towards the goal relative to how far away it is (its hunting strategy)
        walkPoint += Vector3.Normalize(HearGoal(monster))*(Mathf.Max(0,(Vector3.Magnitude(HearGoal(monster))-30))/3);
        walkPoint = monster.SetWithingBounds(walkPoint);

        walkPointSet = true;  
    }

    private Vector3 HearGoal(BlindMonsterStateManager monster) {
        return (Goal.transform.position - monster.transform.position);
    }

}
