using UnityEngine;
using UnityEngine.Events;

public class ExperienceManager : Singleton<ExperienceManager>
{
    public UnityAction OnLevelUp;

    [SerializeField] private SOInt Level;
    [SerializeField] private SOInt EXP;
    [SerializeField] private int _experienceToNextLevel = 10;

    private PlayerStateMachine _player;

    private void Start()
    {
        _player = PlayerStateMachine.Instance;
        Level.Value = 1;
        EXP.Value = 0;
    }

    public void AddExperience(int amount)
    {
        EXP.Value += amount;
        if (EXP.Value >= _experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        EXP.Value -= _experienceToNextLevel;
        _experienceToNextLevel = (int)(_experienceToNextLevel * 1.5f);
        Level.Value++;

        // Melhorar o personagem
        _player.PowerUp(Level.Value);

        OnLevelUp?.Invoke();
    }
}