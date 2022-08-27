using UnityEngine;

public class BlindMonsterStopState : BlindMonsterBaseState
{
    private float restingSeconds = 1f; 

    public override void EnterState(BlindMonsterStateManager monster) {
        Debug.Log("Stop");

        restingSeconds = Random.Range(2f,5f); // Escolhe quanto tempo vai descansar, variando de 2 a 10s
    }

    public override void FixedUpdateState(BlindMonsterStateManager monster) {
        restingSeconds -= Time.deltaTime;
        if (restingSeconds <= 0)
            monster.SwitchState(monster.PatrolState); 

        Debug.Log("Stop" + restingSeconds + "s");
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
}
