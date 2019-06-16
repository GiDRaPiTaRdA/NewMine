using System.Collections;
using System.Collections.Generic;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EditTextScript : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    public TMP_InputField inputText;

    public EventString onAccept;

    public UnityEvent onCancel;

    public UnityEvent onClose;

    public void Accept()
    {
        if (!string.IsNullOrWhiteSpace(this.inputText?.text))
        {
            this.onAccept?.Invoke(this.inputText.text);
        }
    }

    public void Cancel() => this.onCancel?.Invoke();

    public void Close() => this.onClose?.Invoke();
}
