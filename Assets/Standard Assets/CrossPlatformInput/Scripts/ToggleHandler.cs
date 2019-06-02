using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Standard_Assets.CrossPlatformInput.Scripts
{
    public class ToggleHandler : MonoBehaviour
    {
        public string toogleName;

        private CrossPlatformInputManager.VirtualButton ToggleButton { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            this.ToggleButton = new CrossPlatformInputManager.VirtualButton(this.toogleName);

            CrossPlatformInputManager.RegisterVirtualButton(this.ToggleButton);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Toggle(Boolean value)
        {
           
        }
    }
}
