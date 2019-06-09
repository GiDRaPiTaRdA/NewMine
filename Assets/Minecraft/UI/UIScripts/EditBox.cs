using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class EditBox : MonoBehaviour
{
    public bool createMode;
    public EventString create;
    public EventString edit;

    public string text;

    //GameObject

    public void Accept()
    {
        if (this.createMode)
        {
            this.create?.Invoke(this.text);
        }
        else
        {
            this.edit?.Invoke(this.text);
        }
    }
 
    public void SetCreateMode(bool mode)
    {
        this.createMode = mode;
    }
}

[System.Serializable]
public class EventString : UnityEvent<string> { }

[System.Serializable]
public enum EditBoxMode
{
    CreateNew, Edit
}
