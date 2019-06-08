using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;

public class DestroySession : MonoBehaviour
{
    public GameObject goARCoreDevice;

    //void Start()
    //{
    //    this.StartSession();
    //}

    //[Tooltip("A scriptable object specifying the ARCore session configuration.")]
    // public ARCoreSessionConfig SessionConfig;

    //public void StartSession()
    //{
    //    ARCoreSession session = this.goARCoreDevice.AddComponent<ARCoreSession>();
    //    session.SessionConfig = this.SessionConfig;
    //    LifecycleManager.Instance.CreateSession(this.goARCoreDevice.GetComponent<ARCoreSession>());
    //    LifecycleManager.Instance.EnableSession();

    //    LifecycleManager.Instance.
    //}

    public void Dest()
    {
        ARCoreSession session = this.goARCoreDevice.GetComponent<ARCoreSession>();
        DestroyImmediate(session);
    }
}
