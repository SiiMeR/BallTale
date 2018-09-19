using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : SimpleMenu<PauseMenu>
{
    // Use this for initialization
    private void Start()
    {
    }

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

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnQuitPressed()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnOptionsPressed()
    {
        PauseOptionsMenu.Show();
    }
}