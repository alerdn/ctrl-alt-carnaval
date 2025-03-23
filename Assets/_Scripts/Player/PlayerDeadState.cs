using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    private readonly int DeadHash = Animator.StringToHash("Dead");
    private float _respawnDelay = 5f;
    private bool _loading;

    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _loading = false;
        AudioManager.Instance.PlayCue("GameOver");
        stateMachine.Animator.CrossFadeInFixedTime(DeadHash, .1f);
    }

    public override void Tick(float deltaTime)
    {
        _respawnDelay -= deltaTime;

        if (_respawnDelay <= 0 && !_loading)
        {
            _loading = true;
            GameManager.Instance.LoadScene("SCN_Game");
        }
    }

    public override void Exit()
    {
    }
}