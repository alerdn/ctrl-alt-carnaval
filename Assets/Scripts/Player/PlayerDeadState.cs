using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    private readonly int DeadHash = Animator.StringToHash("Dead");
    private float _respawnDelay = 5f;

    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        AudioManager.Instance.PlayCue("GameOver");
        stateMachine.Animator.CrossFadeInFixedTime(DeadHash, .1f);
    }

    public override void Tick(float deltaTime)
    {
        _respawnDelay -= deltaTime;

        if (_respawnDelay <= 0)
        {
            GameManager.Instance.LoadScene("SCN_Game");
        }
    }

    public override void Exit()
    {

    }
}