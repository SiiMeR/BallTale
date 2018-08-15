using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : SimpleMenu<MainMenu>
{
    public Button continueButton;

    // Use this for initialization
    private void Start()
    {
        Time.timeScale = 1;

        if (SaveGame.Exists("player.txt"))
            continueButton.interactable = true;
        else
            PlayerPrefs.SetInt("loadgame", 0);
    }

    private void FixedUpdate()
    {
        if (SaveGame.Exists("player.txt"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
            PlayerPrefs.SetInt("loadgame", 0);
        }
    }

    public override void OnBackPressed()
    {
        Application.Quit();
    }


    public void OnNewGamePressed()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OnContinuePressed()
    {
        PlayerPrefs.SetInt("loadgame", 1);
        SceneManager.LoadScene("Tutorial");
    }

    public void OnOptionsPressed()
    {
        OptionsMenu.Show();
    }

    public void OnCreditsPressed()
    {
        CreditsMenu.Show();
    }
}