using System;
using System.Collections;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class MenuScript : MonoBehaviour
{
    public Light day;
    public Light night;

    public bool dayLight = true;

    public bool gamePaused;

    public UnityEvent saved;

    public void Quit() => Application.Quit();

    public void Pause(Boolean pause)
    {
        Time.timeScale = pause ? 0 : 1;
        this.gamePaused = pause;
    }

    public void ToggleLight(Boolean value)
    {
        this.dayLight = value;

        if (this.dayLight)
        {
            if (this.day != null) this.day.enabled = true;
            if (this.night != null) this.night.enabled = false;
        }
        else
        {
            if (this.day != null) this.day.enabled = false;
            if (this.night != null) this.night.enabled = true;
        }
    }

    public void SaveWorld() => this.StartCoroutine(this.SaveWorldCoroutine());

    public IEnumerator SaveWorldCoroutine()
    {
        yield return null;
        SaveManager.Instance.SaveData(StaticWorld.Instance, "Default");
        yield return null;
        this.saved?.Invoke();
        yield return null;
    }
}