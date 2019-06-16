using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EditBox : MonoBehaviour
{
    public string defaultText;

    public bool createMode;
    public TMP_InputField worldName;

    public EventString create;
    public EventString edit;

    public string Text => this.worldName.text;

    //GameObject

    public void Accept()
    {
        if (this.createMode)
        {
            this.create?.Invoke(this.worldName.text);
        }
        else
        {
            this.edit?.Invoke(this.worldName.text);
        }
    }
 
    public void SetCreateMode(bool mode)
    {
        this.createMode = mode;
    }
}


