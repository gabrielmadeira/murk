using UnityEngine;

public class BlindMonsterPatrolState : BlindMonsterBaseState
{
    private Vector3 walkPoint;
    private bool walkPointSet;

    private float distanceToWalkPoint;
    private float oldDistanceToWalkPoint;

    private float walkAttemptDuration;

    public override void EnterState(BlindMonsterStateManager monster) {
        Debug.Log("Patrol");

        monster.pace = monster.patrolSpeed;
        monster.audioSrc.volume = monster.minVol;
        
        walkPointSet = false;
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {
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
        Debug.Log("Patrolling" + walkPoint);
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

        walkPoint = monster.SetWithingBounds(walkPoint);

        walkPointSet = true;  
    }

}
