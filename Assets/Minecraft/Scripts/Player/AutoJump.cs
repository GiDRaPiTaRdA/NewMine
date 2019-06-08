using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class AutoJump : MonoBehaviour
{
    public FirstPersonController fpsController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 origin = this.fpsController.gameObject.transform.position;
        Vector3 direction = this.fpsController.MoveDirection2D.normalized;
        float distance = Mathf.Sqrt(this.fpsController.MoveDirection2D.magnitude) * 0.7f;

        int layerMask = 1 << 0;

        if (Physics.Raycast(origin, direction, out _, distance,layerMask) &&
            !this.fpsController.IsJumping &&
            !Physics.Raycast(origin + Vector3.up, direction, out _, distance,layerMask))
        {
            this.fpsController.Jump();
        }
    }
}
