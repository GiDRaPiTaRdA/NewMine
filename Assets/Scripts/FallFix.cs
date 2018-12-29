using UnityEngine;

namespace Assets.Scripts
{
    public class FallFix : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (this.transform.position.y<-10)
            {
                this.transform.position = new Vector3(10,50,10);
            }
        }
    }
}
