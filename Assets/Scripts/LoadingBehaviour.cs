using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBehaviour : MonoBehaviour
{

    public Slider slider;
    public GameObject fpsController;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)this.slider.value != (int)this.slider.maxValue)
        {
            this.slider.value = StaticWorld.Instance.loadingPercent;
        }
        else
        {
            this.gameObject.SetActive(false);
            fpsController.SetActive(true);
        }
    }
}
