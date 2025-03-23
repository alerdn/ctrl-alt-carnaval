using UnityEngine;
using UnityEngine.Events;

public class ExperienceManager : Singleton<ExperienceManager>
{
    public UnityAction<int> OnLevelUp;

    public float ExperienceToNextLevel => _experienceToNextLevel;

    [SerializeField] private SOInt Level;
    [SerializeField] private SOFloat EXP;
    [SerializeField] private float _experienceToNextLevel;

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

    public void AddExperience(float amount)
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