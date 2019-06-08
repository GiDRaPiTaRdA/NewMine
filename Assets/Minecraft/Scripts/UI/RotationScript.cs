using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public float speed;


    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0,0,this.speed);
    }
}
