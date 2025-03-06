using UnityEngine;

public abstract class EnemyBaseState : State
{
    protected EnemyStateMachine stateMachine;

    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    // protected void Move(Vector3 motion, float deltaTime)
    // {
    //     Vector3 force = stateMachine.ForceReceiver.Movement;
    //     stateMachine.CharacterController.Move((motion + force) * deltaTime);
    // }

    protected void FacePlayer()
    {
        if (stateMachine.Player == null) return;

        Vector3 lookPos = stateMachine.Player.transform.position - stateMachine.transform.position;
        lookPos.y = 0;

        stateMachine.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    protected bool IsInAttackRange()
    {
        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
        return playerDistanceSqr <= Mathf.Pow(stateMachine.AttackRange, 2);
    }
}