using UnityEngine;

public class BlindMonsterHuntState : BlindMonsterBaseState
{
    public override void EnterState(BlindMonsterStateManager monster) {
        Debug.Log("Hunt");

        monster.pace = monster.huntSpeed;
        monster.audioSrc.volume = 1f;
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {
        Move(monster, monster.playerLastHeardAt);
        Debug.Log("Hunt " + monster.playerLastHeardAt);
    }

    public override void OnTriggerEnterState(BlindMonsterStateManager monster, Collider other) {

    }

    public override void OnTriggerStayState(BlindMonsterStateManager monster, Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            monster.playerLastHeardAt = other.gameObject.transform.position;
        }
    }

    public override void OnTriggerExitState(BlindMonsterStateManager monster, Collider other) {
        if (other.gameObject.tag == "SoundArea") {
            monster.SwitchState(monster.CluelessState);
        }
    }

}
