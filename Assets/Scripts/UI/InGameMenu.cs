using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private GameObject _mainFrame;

    private void Start()
    {
        _inputReader.PauseEvent += TogglePause;

        _mainFrame.SetActive(false);
    }

    private void OnDestroy()
    {
        _inputReader.PauseEvent -= TogglePause;
    }

    private void TogglePause()
    {
        bool isPaused = Time.timeScale == 0;
        if (isPaused)
        {
            Time.timeScale = 1;
            _mainFrame.SetActive(false);
            _inputReader.SetControllerMode(ControllerMode.Gameplay);
        }
        else
        {
            Time.timeScale = 0;
            _mainFrame.SetActive(true);
            _inputReader.SetControllerMode(ControllerMode.UI);
        }
    }

    public void OnClick_Continue()
    {
        TogglePause();
    }

    public void OnClick_Quit()
    {
        TogglePause();
        GameManager.Instance.LoadScene("SCN_Menu");
    }
}
