using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private GameObject _loadingFrame;
    [SerializeField] private TMP_Text _loadingText;

    private AsyncOperation asyncLoad;
    private bool _loadHasFinished;

    private void Start()
    {
        _loadingFrame.SetActive(false);

        _inputReader.InteractEvent += ConfirmLoad;
    }

    private void OnDestroy()
    {
        _inputReader.InteractEvent -= ConfirmLoad;
    }

    public void LoadScene(string sceneName)
    {
        _inputReader.SetControllerMode(ControllerMode.UI);

        _loadingFrame.SetActive(true);
        _loadingText.text = "Carregando...";

        bool waitForConfirmation = sceneName != "SCN_Menu";
        StartCoroutine(LoadSceneRoutine(sceneName, waitForConfirmation));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, bool waitForConfirmation)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                if (waitForConfirmation)
                {
                    _loadingText.text = "Espaco para continuar";
                    _loadHasFinished = true;
                }
                else
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    private void ConfirmLoad()
    {
        if (!_loadHasFinished) return;

        asyncLoad.allowSceneActivation = true;
    }
}