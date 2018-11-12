using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{

    [SerializeField] private GameObject _deathScreen;
    
    public IEnumerator Death()
    {
        _deathScreen.SetActive(true);

        AudioManager.Instance.StopAllMusic();
        AudioManager.Instance.SetSoundVolume(0);

        Time.timeScale = 0.0f;
        
        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));

        Time.timeScale = 1.0f;
        if (SaveGame.Exists("player.txt"))
        {
            PlayerPrefs.SetInt("loadgame", 1);
        }

        _deathScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
