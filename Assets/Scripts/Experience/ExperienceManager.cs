using UnityEngine;

public class ExperienceManager : Singleton<ExperienceManager>
{
    [SerializeField] private SOInt EXP;
    [SerializeField] private int _experienceToNextLevel = 100;

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
        _experienceToNextLevel = (int)(_experienceToNextLevel * 1.1f);
    }

}