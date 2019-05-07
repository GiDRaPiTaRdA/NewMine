using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{

    public GameObject cam;

    public BlockType blockType = BlockType.DIRT;

    // Update is called once per frame
    void Update()
    {
        this.MouseInputs();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.blockType = BlockType.GRASS;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.blockType = BlockType.DIRT;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            this.blockType = BlockType.STONE; 
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            this.blockType = BlockType.GLASS;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            this.blockType = BlockType.GLOWSTONE;
        }
    }

  

    private void MouseInputs()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //for cross airs
            if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out RaycastHit hit, 10))
            {
                if (!StaticWorld.Instance.Chunks.TryGetValue(hit.collider.gameObject.transform.position, out Chunk hitc)) return;
              
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 hitPos = hit.point - hit.normal / 2.0f;
                    Vector3 hitBlock = new Vector3(
                        (int)(Mathf.Round(hitPos.x) - hit.collider.gameObject.transform.position.x),
                        (int)(Mathf.Round(hitPos.y) - hit.collider.gameObject.transform.position.y),
                        (int)(Mathf.Round(hitPos.z) - hit.collider.gameObject.transform.position.z));

                    //Debug.Log(hit.point);

                    //this.RemoveBlock(hitBlock, hitc);

                    //Debug.Log(hitBlock);

                    StaticWorld.Instance.StartCoroutine(hitc.RemoveBlock(hitBlock));



                }
                else
                {
                    Vector3 hitPos = hit.point + hit.normal / 2.0f;

                    Vector3 hitBlock = new Vector3(
                        (int)(Mathf.Round(hitPos.x) - hit.collider.gameObject.transform.position.x),
                        (int)(Mathf.Round(hitPos.y) - hit.collider.gameObject.transform.position.y),
                        (int)(Mathf.Round(hitPos.z) - hit.collider.gameObject.transform.position.z));

                    StaticWorld.Instance.StartCoroutine(hitc.AddBlock(hitBlock,this.blockType));
                }

            }
        }
    }
}


