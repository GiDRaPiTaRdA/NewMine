using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
#if UNITY_STANDALONE_WIN
        Application.targetFrameRate = 60;
#elif MOBILE_INPUT
        Application.targetFrameRate = 30;
#endif
    }

    public void Quit() => Application.Quit();
    public void MainMenu() => SceneManager.LoadScene(0);
    public void PlayFps() =>  SceneManager.LoadSceneAsync(1);
    public void PlayAr() => SceneManager.LoadSceneAsync(2);


}
