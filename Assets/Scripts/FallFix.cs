using UnityEngine;

namespace Assets.Scripts
{
    public class FallFix : MonoBehaviour
    {
        public int height = 50;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (this.transform.position.y<-10)
            {
                if (this.transform.position.x > 0 && this.transform.position.z > 0 &&
                    this.transform.position.x < World.chunkSize * StaticWorld.Instance.worldSize &&
                    this.transform.position.z < World.chunkSize * StaticWorld.Instance.worldSize)
                    this.transform.position = new Vector3(this.transform.position.x, this.height, this.transform.position.z);
                else
                {
                    this.transform.position = new Vector3(World.chunkSize * StaticWorld.Instance.worldSize / 2, this.height, World.chunkSize * StaticWorld.Instance.worldSize / 2);
                }
            }
          
        }
    }
}
