using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour
{
    public bool isOn;

    public GameObject imageOn;
    public GameObject imageOff;

    public ToggleEvent onValueChanged;
    public ToggleOperrtationEvent onValueOn;
    public ToggleOperrtationEvent onValueOff;

    void Start()
    {
        this.imageOn.SetActive(this.isOn);
        this.imageOff.SetActive(!this.isOn);
    }

    public void ToggleChanged()
    {
        this.isOn = !this.isOn;
        this.onValueChanged.Invoke(this.isOn);
        if (this.isOn)
        {
            this.onValueOn.Invoke();
        }
        else
        {
            this.onValueOff.Invoke();
        }
    }

    /// <summary>
    ///   <para>Event type used by the UI.Slider.</para>
    /// </summary>
    [Serializable]
    public class ToggleEvent : UnityEvent<bool> { }

    /// <summary>
    ///   <para>Event type used by the UI.Slider.</para>
    /// </summary>
    [Serializable]
    public class ToggleOperrtationEvent : UnityEvent { }



    public Transform transform { get; }
}
