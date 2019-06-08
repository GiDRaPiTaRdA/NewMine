using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Virtual_Joystick_Pack.Scripts
{
    public class AxisJoystic : MonoBehaviour
    {
        public Joystick joystick;

        public float multiplier = 1f;

        public string horizontalAxisName = "Horizontal";
        public string verticalsAxisName = "Vertical";

        private CrossPlatformInputManager.VirtualAxis HorizontalAxis { get; set; }
        private CrossPlatformInputManager.VirtualAxis VerticalAxis { get; set; }


        // Start is called before the first frame update
        void Start()
        {
            if (CrossPlatformInputManager.AxisExists(this.horizontalAxisName))
                CrossPlatformInputManager.UnRegisterVirtualAxis(this.horizontalAxisName);

            if (CrossPlatformInputManager.AxisExists(this.verticalsAxisName))
                CrossPlatformInputManager.UnRegisterVirtualAxis(this.verticalsAxisName);

            this.HorizontalAxis = new CrossPlatformInputManager.VirtualAxis(this.horizontalAxisName);
            this.VerticalAxis = new CrossPlatformInputManager.VirtualAxis(this.verticalsAxisName);

            CrossPlatformInputManager.RegisterVirtualAxis(this.HorizontalAxis);
            CrossPlatformInputManager.RegisterVirtualAxis(this.VerticalAxis);
        }

        // Update is called once per frame
        void Update()
        {
            //float deadArea = 0f;

            //float h = 0;
            //float v = 0;

            //if (this.joystick.Horizontal > deadArea|| this.joystick.Horizontal < -deadArea)
            //{
            //    h = this.joystick.Horizontal - deadArea;
            //}
            //if (this.joystick.Vertical > deadArea|| this.joystick.Vertical < -deadArea)
            //{
            //    v = this.joystick.Vertical - deadArea;
            //}

            this.HorizontalAxis.Update(this.joystick.Horizontal * this.multiplier);
            this.VerticalAxis.Update(this.joystick.Vertical * this.multiplier);
        }




    }
}
