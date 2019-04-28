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
        Application.targetFrameRate = 60;
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
