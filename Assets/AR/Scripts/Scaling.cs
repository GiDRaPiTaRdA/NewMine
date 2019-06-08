using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scaling : MonoBehaviour
{
    public int originWidth = 2960;
    public int originHeight = 1440;

    private float scaleCoficientWidth;
    private float scaleCoficientHeight;

    // Use this for initialization
    void Start()
    {
        this.scaleCoficientWidth = (float)Screen.width / this.originWidth;
        this.scaleCoficientHeight = (float)Screen.height / this.originHeight;

        // Apply scale
        var scaleFactor = (this.scaleCoficientHeight + this.scaleCoficientWidth) / 2;

        this.gameObject.GetComponent<CanvasScaler>().scaleFactor *= scaleFactor;
    }

}
