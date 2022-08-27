using UnityEngine;

public class BlindMonsterPatrolState : BlindMonsterBaseState
{
    private Vector3 walkPoint;
    private bool walkPointSet = false;


    public override void EnterState(BlindMonsterStateManager monster) {
        Debug.Log("Patrol");

        monster.pace = monster.patrolSpeed;
        monster.audioSrc.volume = 0.04f;
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {
        if (!walkPointSet)
            SearchWalkPoint(monster); // Finds a new walkpoint
        else {
            Vector3 distanceToWalkPoint = monster.transform.position-walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
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
            monster.SwitchState(monster.HuntState);
        }
    }

    public override void OnTriggerStayState(BlindMonsterStateManager monster, Collider other) {

    }

    public override void OnTriggerExitState(BlindMonsterStateManager monster, Collider other) {

    }

    private void SearchWalkPoint(BlindMonsterStateManager monster) {
        // Creates a new point to walk to 
        Vector2 randomXZ = Random.insideUnitCircle * 45;
        randomXZ[0] += monster.transform.position.x;
        randomXZ[1] += monster.transform.position.z;
        
        monster.SetWithingBounds(randomXZ);

        walkPoint = new Vector3(randomXZ[0], monster.transform.position.y, randomXZ[1]);
        walkPointSet = true;  
    }

}
