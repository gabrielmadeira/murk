using UnityEngine;

public class BlindMonsterHuntState : BlindMonsterBaseState
{
    private float distanceFromPlayer;
    private float timePersuing;

    public override void EnterState(BlindMonsterStateManager monster) {
        //Debug.Log("Hunt");

        monster.certainty = Mathf.Max(0.05f,monster.certainty);

        // Gets the distance from player being chased
        distanceFromPlayer = Vector3.Distance(monster.transform.position, monster.playerLastHeardAt);

        // Adjusts the scale of the uncertainty zone display
        monster.uncertaintyZoneDiameter = distanceFromPlayer*(2-monster.certainty);
        monster.uncertaintyZonePrefab.transform.localScale = new Vector3(monster.uncertaintyZoneDiameter, 0.1f, monster.uncertaintyZoneDiameter);

        // Creates the uncertainty zone
        monster.uncertaintyZone = Object.Instantiate(monster.uncertaintyZonePrefab, monster.playerLastHeardAt, Quaternion.Euler(0, 0, 0)); //, monster.itself.transform);
        
        // Gives it a name
        monster.uncertaintyZone.name = monster.name + " Uncertainty Zone";
        
        // Adjusts the height of the uncertainty zone display (DUMB CODE)
        monster.uncertaintyZone.transform.position = new Vector3(monster.playerLastHeardAt.x, 0.15f, monster.playerLastHeardAt.z);
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {
        // Makes the monster more certain of the players position (takes about 5s to reach maximum certainty)
        monster.certainty += Time.deltaTime/5;
        if (monster.certainty > 1)
            monster.certainty = 1;
        
        monster.pace = monster.patrolSpeed + (monster.huntSpeed-monster.patrolSpeed)*monster.certainty; // Adjusts the monster's speed based on his certainty
        monster.audioSrc.volume = monster.minVol + (1-monster.minVol)*monster.certainty; // Adjusts the monster's volume based on his certainty

        Move(monster, monster.playerLastHeardAt);

        //Debug.Log("Hunt " + monster.playerLastHeardAt + " Certainty: " + monster.certainty);
    }

    public override void OnTriggerEnterState(BlindMonsterStateManager monster, Collider other) {

    }

    public override void OnTriggerStayState(BlindMonsterStateManager monster, Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            monster.playerLastHeardAt = other.gameObject.transform.position;

            // Gets the distance from player being chased
            distanceFromPlayer = Vector3.Distance(monster.transform.position, monster.playerLastHeardAt);

            // Adjusts the scale of the uncertainty zone display
            monster.uncertaintyZoneDiameter = distanceFromPlayer*(2-monster.certainty);
            monster.uncertaintyZone.transform.localScale = new Vector3(monster.uncertaintyZoneDiameter, 0.1f, monster.uncertaintyZoneDiameter);

            // Adjusts the position of the uncertainty zone display
            monster.uncertaintyZone.transform.position = new Vector3(monster.playerLastHeardAt.x, 0.15f, monster.playerLastHeardAt.z);
        }
    }

    public override void OnTriggerExitState(BlindMonsterStateManager monster, Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            monster.SwitchState(monster.CluelessState);
        }
    }

}
