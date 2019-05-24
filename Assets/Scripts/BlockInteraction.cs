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
                hit.point = StaticWorld.Instance.gameObject.transform.InverseTransformPoint(hit.point);
                hit.normal = StaticWorld.Instance.gameObject.transform.InverseTransformPoint(hit.normal+StaticWorld.Instance.gameObject.transform.position);

                bool inChunkHit = false;

                GameObject hitGameObject = hit.collider.gameObject;

                Position? hitPosition = hitGameObject.GetComponent<ObjectPosition>()?.Position;

                Chunk hitc = null;

                if (hitPosition != null)
                {
                    // Find chunk by hit tranform
                    bool res = StaticWorld.Instance.Chunks.TryGetValue(hitPosition.Value, out hitc);

                    if (res)
                    {
                        inChunkHit = true;
                    }
                    else
                    {
                        // Find Chunk by block position
                        //hitGameObject.transform.parent.GetComponent<Chunk>()

                        ////Position pos = StaticWorld.GetChunkPosition(StaticWorld.Instance.gameObject.transform.InverseTransformPoint(hitGameObject.transform.position) / StaticWorld.K);

                        StaticWorld.Instance.Chunks.TryGetValue(hitGameObject.transform.parent.GetComponent<ObjectPosition>().Position, out hitc);
                    }
                }

                if (hitPosition == null)
                {
                    Debug.Log("Unknown object");
                    return;
                }


                // Remove Block
                if (Input.GetMouseButtonDown(0))
                {
                    this.RemoveBlock(hit, hitc, hitPosition.Value, inChunkHit);
                }

                // Add Block
                else
                {
                    this.AddBlock(hit, hitc);
                }

            }
        }
    }

    private void AddBlock(RaycastHit hit, Chunk hitc)
    {
        Vector3 hitPos = hit.point + hit.normal * StaticWorld.K / 2.0f;

        Position hitBlock = (Position)Round(hitPos / StaticWorld.K) - hitc.Position;

        Block block = StaticWorld.GetWorldBlock(hitBlock + hitc.Position);


        if (block?.Type == BlockType.AIR)
        {
            Debug.Log($"ADD Chunk {hitc.Position} Block {hitBlock} OLD {block.Type} NEW {this.blockType}");
            StaticWorld.Instance.StartCoroutine(hitc.AddBlock(block, this.blockType));
        }
        else
        {
            Debug.Log("Can not add block here");
        }
    }

    private void RemoveBlock(RaycastHit hit, Chunk hitc, Position hitBlockPosition, bool inChunkHit)
    {
        if (inChunkHit)
        {
            Vector3 hitPos = hit.point - hit.normal * StaticWorld.K / 2.0f;

            hitBlockPosition = (Position)Round(hitPos / StaticWorld.K) - hitc.Position;

            Debug.Log(hitBlockPosition);
        }

        Block b = hitc.ChunkData[hitBlockPosition];

        Debug.Log($"REVOVE Chunk {hitc.Position} Block {hitBlockPosition} Type {b.Type}");

        StaticWorld.Instance.StartCoroutine(hitc.RemoveBlock(b));
        //StaticWorld.Instance.StartCoroutine(hitc.UpdateBlock(b, BlockType.AIR, hitBlockPosition));
    }

    private static Vector3 Round(Vector3 vector3) => new Vector3(
        Mathf.Round(vector3.x),
        Mathf.Round(vector3.y),
        Mathf.Round(vector3.z));
}


