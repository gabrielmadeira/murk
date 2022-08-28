using UnityEngine;

public class BlindMonsterPatrolState : BlindMonsterBaseState
{
    private Vector3 walkPoint;
    private bool walkPointSet = false;


    public override void EnterState(BlindMonsterStateManager monster) {
        Debug.Log("Patrol");

        monster.pace = monster.patrolSpeed;
        monster.audioSrc.volume = monster.minVol;
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
