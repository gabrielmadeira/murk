using UnityEngine;

public abstract class BlindMonsterBaseState
{
    public abstract void EnterState(BlindMonsterStateManager monster);

    public abstract void FixedUpdateState(BlindMonsterStateManager monster);

    public abstract void OnTriggerEnterState(BlindMonsterStateManager monster, Collider other);

    public abstract void OnTriggerStayState(BlindMonsterStateManager monster, Collider other);

    public abstract void OnTriggerExitState(BlindMonsterStateManager monster, Collider other);


    public void Move(BlindMonsterStateManager monster, Vector3 walkPoint) {
        // Moves the monster
        // Debug.Log("Walk Point = " + walkPoint);
        Vector3 distanceToWalkPoint = monster.transform.position-walkPoint;

        //find target velocity
        Vector3 currentVelocity = monster.rb.velocity;
        Vector3 targetVelocity = new Vector3(-distanceToWalkPoint.x,0,-distanceToWalkPoint.z);

        targetVelocity = targetVelocity.normalized*monster.pace;

        //Align direction
        targetVelocity = monster.transform.TransformDirection(targetVelocity);

        //Calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange.y = 0;

        //Limit force
        Vector3.ClampMagnitude(velocityChange, 1f);

        monster.rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
