using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AcceptScript : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    public UnityEvent onAccept;

    public UnityEvent onCancel;

    public UnityEvent onClose;

    public void Accept()=> this.onAccept?.Invoke();

    public void Cancel() => this.onCancel?.Invoke();

    public void Close() => this.onClose?.Invoke();
}
