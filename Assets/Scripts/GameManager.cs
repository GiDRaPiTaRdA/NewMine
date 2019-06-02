using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Light day;
    public Light night;

    public bool dayLight = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
#if UNITY_STANDALONE_WIN
        Application.targetFrameRate = 60;
#elif MOBILE_INPUT
        Application.targetFrameRate = 30;
#endif

    }

    public void ToggleLight(Boolean value)
    {
        this.dayLight = value;

        if (this.dayLight)
        {
            this.day.enabled = true;
            this.night.enabled = false;
        }
        else
        {
            this.day.enabled = false;
            this.night.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_WIN
       
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.dayLight = !this.dayLight;

            if (this.dayLight)
            {
                this.day.enabled = true;
                this.night.enabled = false;
            }
            else
            {
                this.day.enabled = false;
                this.night.enabled = true;
            }
        }
#endif
    }
}
