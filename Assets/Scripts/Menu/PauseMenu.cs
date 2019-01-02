using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : SimpleMenu<PauseMenu>
{
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        Time.timeScale = 1;
    }

    public void OnQuitPressed()
    {
        SaveGameManager.Instance.CreateSaveGame();
        SceneManager.LoadScene("Menu");
    }

    public void OnOptionsPressed()
    {
        PauseOptionsMenu.Show();
    }
}