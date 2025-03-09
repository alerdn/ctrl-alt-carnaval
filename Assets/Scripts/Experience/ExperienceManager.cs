using UnityEngine;
using UnityEngine.Events;

public class ExperienceManager : Singleton<ExperienceManager>
{
    public UnityAction<int> OnLevelUp;

    public int ExperienceToNextLevel => _experienceToNextLevel;

    [SerializeField] private SOInt Level;
    [SerializeField] private SOInt EXP;
    [SerializeField] private int _experienceToNextLevel;

    private PlayerStateMachine _player;


    protected override void Awake()
    {
        base.Awake();

        Level.Value = 1;
        EXP.Value = 0;
    }

    private void Start()
    {
        _player = PlayerStateMachine.Instance;
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
        Level.Value++;
        EXP.Value -= _experienceToNextLevel;

        // Melhorar o personagem
        _player.PowerUp(Level.Value);

        OnLevelUp?.Invoke(Level.Value);
    }
}