using UnityEngine;

public class BlindMonsterCluelessState : BlindMonsterBaseState
{
    private Vector3 walkPoint;
    private bool walkPointSet;

    private float attentionSpam;

    public override void EnterState(BlindMonsterStateManager monster) {
        Debug.Log("Clueless");

        monster.pace = monster.patrolSpeed + (monster.huntSpeed-monster.patrolSpeed)*attentionSpam/15; // Gradually desacelerates the monster
        monster.audioSrc.volume = 0.04f + 0.96f*attentionSpam/15; // Gradually makes the monster more silent

        walkPointSet = false;
        attentionSpam = Random.Range(15f,25f);
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {
        attentionSpam -= Time.deltaTime;

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
        Vector2 randomXZ = Random.insideUnitCircle * 17;
        randomXZ[0] += monster.playerLastHeardAt.x;
        randomXZ[1] += monster.playerLastHeardAt.z;

        monster.SetWithingBounds(randomXZ);

        walkPoint = new Vector3(randomXZ[0], monster.transform.position.y, randomXZ[1]);
        Debug.Log("Cluless" + walkPoint);
        walkPointSet = true;  
    }
}
