using System;
using System.Collections;
using System.Collections.Generic;
using AR;
using GoogleARCore;
using UnityEngine;

public class ArPlanesContoller : MonoBehaviour
{
    public ARController arController;
    public ARCoreSession arSession;

    public void TogglePlanes(Boolean enable)
    {
        arSession.SessionConfig.EnablePlaneFinding = enable;
        arSession.OnEnable();
        this.arController.planes.ForEach(p=>p.SetActive(enable));
    }
}
