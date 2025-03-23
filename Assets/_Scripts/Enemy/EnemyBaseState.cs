using UnityEngine;

public abstract class EnemyBaseState : State
{
    protected EnemyStateMachine stateMachine;

    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void MoveToPlayer()
    {
        MoveTo(stateMachine.Player.transform.position);
    }

    protected void MoveTo(Vector3 position)
    {
        if (Time.frameCount % 10 != 0) return;

        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = position;
        }
    }

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

    protected bool IsInImpactRange()
    {
        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
        return playerDistanceSqr <= Mathf.Pow(stateMachine.ImpactRange, 2);
    }
}