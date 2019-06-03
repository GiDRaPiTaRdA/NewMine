using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public bool gamePaused;

    public void Quit()=> Application.Quit();

    public void Pause()
    {
        if (!this.gamePaused)
        {
            Time.timeScale = 0;
            this.gamePaused = true;
        }
    }

    public void Resume()
    {
        if (this.gamePaused)
        {
            Time.timeScale = 1;
            this.gamePaused = false;
        }
    }

    public void MainMenu() => SceneManager.LoadScene(0);
    public void Play() => SceneManager.LoadScene(1);
}
