using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBehaviour : MonoBehaviour
{

    public Slider slider;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnStartButtonClick()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)this.slider.value != (int)this.slider.maxValue)
        {
            this.slider.value = StaticWorld.Instance.loadingPercent;
        }
        else if(!this.button.IsActive())
        {
#if UNITY_STANDALONE_WIN
            this.gameObject.SetActive(false);
#elif MOBILE_INPUT
            this.button.gameObject.SetActive(true);
#endif


        }
    }
}
