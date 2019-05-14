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

        if (Input.GetKeyDown(KeyCode.Alpha1)) this.blockType = (BlockType)0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) this.blockType = (BlockType)1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) this.blockType = (BlockType)2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) this.blockType = (BlockType)3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) this.blockType = (BlockType)4;
        if (Input.GetKeyDown(KeyCode.Alpha6)) this.blockType = (BlockType)5;
        if (Input.GetKeyDown(KeyCode.Alpha7)) this.blockType = (BlockType)6;
        if (Input.GetKeyDown(KeyCode.Alpha8)) this.blockType = (BlockType)7;
        if (Input.GetKeyDown(KeyCode.Alpha9)) this.blockType = (BlockType)8;
        if (Input.GetKeyDown(KeyCode.Alpha0)) this.blockType = (BlockType)9;
        if (Input.GetKeyDown(KeyCode.Keypad1)) this.blockType = (BlockType)10;
        if (Input.GetKeyDown(KeyCode.Keypad2)) this.blockType = (BlockType)11;
    }



    private void MouseInputs()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //for cross airs
            if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out RaycastHit hit, 10))
            {
                bool inChunkHit;

                Vector3 pos = Vector3.zero;

                // Find chunk by hit tranform
                if (!StaticWorld.Instance.Chunks.TryGetValue(hit.collider.gameObject.transform.position, out Chunk hitc))
                {
                    // Find Chunk by block position
                    pos = StaticWorld.GetChunkPosition(hit.collider.gameObject.transform.position);

                    if (!StaticWorld.Instance.Chunks.TryGetValue(pos, out hitc)) return;
                    {
                        inChunkHit = false;
                    }
                }
                else
                {
                    inChunkHit = true;
                }


                // Remove Block
                if (Input.GetMouseButtonDown(0))
                {
                    if (inChunkHit)
                    {
                        Vector3 hitPos = hit.point - hit.normal / 2.0f;
                        Vector3 hitBlock = new Vector3(
                            (int) (Mathf.Round(hitPos.x) - hit.collider.gameObject.transform.position.x),
                            (int) (Mathf.Round(hitPos.y) - hit.collider.gameObject.transform.position.y),
                            (int) (Mathf.Round(hitPos.z) - hit.collider.gameObject.transform.position.z));

                        StaticWorld.Instance.StartCoroutine(hitc.RemoveBlock(hitBlock));
                    }
                    else
                    {
                        Vector3 p = hit.collider.gameObject.transform.position - pos;

                        StaticWorld.Instance.StartCoroutine(hitc.RemoveBlock(p));
                    }
                }

                // Add Block
                else
                {
                    Vector3 hitPos = hit.point + hit.normal / 2.0f;

                    Vector3 hitBlock = new Vector3(
                        (int)(Mathf.Round(hitPos.x) - hit.collider.gameObject.transform.position.x),
                        (int)(Mathf.Round(hitPos.y) - hit.collider.gameObject.transform.position.y),
                        (int)(Mathf.Round(hitPos.z) - hit.collider.gameObject.transform.position.z));


                    Block block = StaticWorld.GetWorldBlock(hitBlock + hitc.Position);

                    if (block?.Type == BlockType.AIR)
                    {
                        //StaticWorld.Instance.StartCoroutine(hitc.AddBlock(hitBlock, this.blockType));
                        StaticWorld.Instance.StartCoroutine(hitc.AddBlock(block,hitBlock, this.blockType));
                    }
                    else
                    {
                        Debug.Log("Can not add block here");
                    }

                }

            }
        }
    }
}


