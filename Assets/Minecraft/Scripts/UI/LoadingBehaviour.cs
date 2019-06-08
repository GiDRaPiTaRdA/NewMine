using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingBehaviour : MonoBehaviour
{

    private readonly Queue<Action> executionQueue = new Queue<Action>();

    public Slider slider;

    public TextMeshProUGUI text;

    public int delay = 0;

    public UnityEvent loadingComplete;

    private bool stopUpdate = false;

    // Update is called once per frame
    void Update()
    {
        if ((int)this.slider.value != (int)this.slider.maxValue)
        {
            this.slider.value = StaticWorld.Instance.loadingPercent;
            this.text.text = $"{this.slider.value}%";
        }
        else
        {
            if (!this.stopUpdate)
            {
                this.stopUpdate = true;

                this.StartCoroutine(this.LoadingComplete());
            }
        }

        foreach (Action action in this.executionQueue)
        {
            action.Invoke();
        }
    }

    void LComplete()
    {
        this.loadingComplete?.Invoke();
        this.gameObject.SetActive(false);
    }

    IEnumerator LoadingComplete()
    {
        yield return new WaitForSeconds(this.delay);

        this.executionQueue.Enqueue(this.LComplete);
    }
}
