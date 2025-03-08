using UnityEngine;

public class PlayerWinState : PlayerBaseState
{
    private readonly int WinHash = Animator.StringToHash("Win");
    private float _respawnDelay = 5f;
    private bool _loading;

    public PlayerWinState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _loading = false;
        AudioManager.Instance.PlayCue("LevelUp");
        stateMachine.Animator.CrossFadeInFixedTime(WinHash, .1f);
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