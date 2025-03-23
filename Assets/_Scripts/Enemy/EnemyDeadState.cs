using System.Collections;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.EXPPool.Get();
        stateMachine.Animator.gameObject.SetActive(false);
        stateMachine.StartCoroutine(DieRoutine());
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(1f);
        if (stateMachine.EnemyPool == null) Object.Destroy(stateMachine.gameObject);
        else stateMachine.EnemyPool.Release(stateMachine);
    }
}