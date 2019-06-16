using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject acceptPrefab;

    public GameObject editTextPrefab;

    public static PopUpManager Instance { get; private set; }

    public PopUpManager()
    {
        Instance = this;
    }


    public void Show(string text,Action action)
    {
        GameObject accept = Instantiate(this.acceptPrefab, this.transform);
        AcceptScript acceptScript = accept.GetComponent<AcceptScript>();
        acceptScript.textMeshPro.text = text;
        acceptScript.onAccept.AddListener(action.Invoke);
        acceptScript.onClose.AddListener(()=>Destroy(accept));
    }

    public void ShowEditField(string text,string textInput, Action<string> action)
    {
        GameObject accept = Instantiate(this.editTextPrefab, this.transform);
        EditTextScript acceptScript = accept.GetComponent<EditTextScript>();
        acceptScript.textMeshPro.text = text;
        acceptScript.inputText.text = textInput;
        acceptScript.onAccept.AddListener(action.Invoke);
        acceptScript.onClose.AddListener(() => Destroy(accept));
    }
}
