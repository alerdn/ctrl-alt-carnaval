using UnityEngine;

public class TitleMenu : MonoBehaviour
{
    public void OnClick_Play()
    {
        GameManager.Instance.LoadScene("SCN_Game");
    }

    public void OnClick_Quit()
    {
        Application.Quit();
    }
}