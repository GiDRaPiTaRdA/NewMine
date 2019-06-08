using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;
using UnityEngine.Events;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;

public class FpsMinecraftInputs : MonoBehaviour
{
    public Light day;
    public Light night;

    public bool dayLight = true;

    public bool gamePaused;

    public UnityEvent saved;

    public void Quit() => Application.Quit();

    public void Pause(Boolean pause)
    {
        Time.timeScale = pause? 0:1;
        this.gamePaused = pause;
    }

    public void ToggleLight(Boolean value)
    {
        this.dayLight = value;

        if (this.dayLight)
        {
            this.day.enabled = true;
            this.night.enabled = false;
        }
        else
        {
            this.day.enabled = false;
            this.night.enabled = true;
        }
    }

    public void SaveWorld()
    {
        SaveManager.Instance.SaveData(StaticWorld.Instance, "Default");
        this.saved?.Invoke();
    }
}
