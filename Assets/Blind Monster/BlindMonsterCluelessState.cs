using UnityEngine;

public class BlindMonsterCluelessState : BlindMonsterBaseState
{
    private Vector3 walkPoint;
    private bool walkPointSet;

    private float attentionSpam;
    private float startingAttentionSpam;

    public override void EnterState(BlindMonsterStateManager monster) {
        //Debug.Log("Clueless");

        walkPointSet = false;
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {
        monster.certainty -= Time.deltaTime/20; // Reduces the certainty of the monster as time passes
        if (monster.certainty < 0)
        {
            Object.Destroy(monster.uncertaintyZone); // Destroys Uncertainty zone
            monster.SwitchState(monster.PatrolState);
        }

        monster.pace = monster.patrolSpeed + (monster.huntSpeed-monster.patrolSpeed)*monster.certainty; // Adjusts the monster's speed based on his certainty
        monster.audioSrc.volume = monster.minVol + (1-monster.minVol)*monster.certainty; // Adjusts the monster's volume based on his certainty

        // Increases the uncertaintyZone cause it has less idea of where the player is
        monster.uncertaintyZoneDiameter += 1.5f*Time.deltaTime;
        monster.uncertaintyZone.transform.localScale = new Vector3(monster.uncertaintyZoneDiameter, 0.1f, monster.uncertaintyZoneDiameter);

        if (!walkPointSet)
            SearchWalkPoint(monster); // Finds a new walkpoint
        else {
            Vector3 distanceToWalkPoint = monster.transform.position-walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
            {
                walkPointSet = false;
            }
        }
        Move(monster, walkPoint);
        //Debug.Log("Cluless" + walkPoint + " Certainty: " + monster.certainty);
    }

    public override void OnTriggerEnterState(BlindMonsterStateManager monster, Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            Object.Destroy(monster.uncertaintyZone); // Destroys Uncertainty zone before making a new one

            float error = Vector3.Distance(monster.playerLastHeardAt, other.gameObject.transform.position); // Gets error in the AI's prediction of the player position
            if (error > monster.uncertaintyZoneDiameter/2) // Adjusts the certainty according to the error
                monster.certainty = Mathf.Max(0,(monster.uncertaintyZoneDiameter-error)/monster.uncertaintyZoneDiameter);

            monster.playerLastHeardAt = other.gameObject.transform.position;
            monster.SwitchState(monster.HuntState);
        }
    }

    public override void OnTriggerStayState(BlindMonsterStateManager monster, Collider other) {

    }

    public override void OnTriggerExitState(BlindMonsterStateManager monster, Collider other) {

    }

    private void SearchWalkPoint(BlindMonsterStateManager monster) {
        // Creates a new point to walk to 
        walkPoint = new Vector3(monster.playerLastHeardAt.x, monster.transform.position.y, monster.playerLastHeardAt.z);
        Vector2 randomXZ = Random.insideUnitCircle * monster.uncertaintyZoneDiameter/2;
        walkPoint.x += randomXZ[0];
        walkPoint.z += randomXZ[1];

        walkPoint = monster.SetWithingBounds(walkPoint);

        walkPointSet = true;  
    }
}
