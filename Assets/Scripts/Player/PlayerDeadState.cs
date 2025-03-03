using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    private float _respawnDelay = 2f;

    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        AudioManager.Instance.PlayCue("GameOver");
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