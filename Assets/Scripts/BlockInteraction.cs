using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{

    public GameObject cam;

    // Update is called once per frame
    void Update()
    {
        this.MouseInputs();

        //this.Input1();
    }

    private void MouseInputs()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //for cross airs
            if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out RaycastHit hit, 10))
            {
                if (!StaticWorld.chunks.TryGetValue(hit.collider.gameObject.transform.position, out Chunk hitc)) return;

              
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

                    StaticWorld.Instance.StartCoroutine(hitc.AddBlock(hitBlock));
                }

            }
        }
    }

    //private IEnumerator RemoveBlock(Vector3 blockPos, Chunk hitChunck)
    //{
    //    Vector3 globalsPos = blockPos + hitChunck.chunk.transform.position;

    //    Block block = StaticWorld.GetWorldBlock(globalsPos);

    //    block.SetType(BlockType.AIR);

    //    hitChunck.ReDrawChunk();

    //    float thisChunkx = hitChunck.chunk.transform.position.x;
    //    float thisChunky = hitChunck.chunk.transform.position.y;
    //    float thisChunkz = hitChunck.chunk.transform.position.z;

    //    //updates.Add(hit.collider.gameObject.name);
    //    List<Vector3> updates = new List<Vector3>();
    //    //update neighbours?
    //    if (blockPos.x == 0)
    //        updates.Add(new Vector3(thisChunkx - World.chunkSize, thisChunky, thisChunkz));
    //    if (blockPos.x == World.chunkSize - 1)
    //        updates.Add(new Vector3(thisChunkx + World.chunkSize, thisChunky, thisChunkz));
    //    if (blockPos.y == 0)
    //        updates.Add(new Vector3(thisChunkx, thisChunky - World.chunkSize, thisChunkz));
    //    if (blockPos.y == World.chunkSize - 1)
    //        updates.Add(new Vector3(thisChunkx, thisChunky + World.chunkSize, thisChunkz));
    //    if (blockPos.z == 0)
    //        updates.Add(new Vector3(thisChunkx, thisChunky, thisChunkz - World.chunkSize));
    //    if (blockPos.z == World.chunkSize - 1)
    //        updates.Add(new Vector3(thisChunkx, thisChunky, thisChunkz + World.chunkSize));

    //    foreach (Vector3 cname in updates)
    //    {
    //        if (StaticWorld.chunks.TryGetValue(cname, out Chunk c))
    //        {
    //            c.ReDrawChunk();
    //        }
    //    }

    //    yield return null;
    //}

    private void Input1()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            //for mouse clicking
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            //if ( Physics.Raycast (ray,out hit,10)) 
            //{

            //for cross hairs
            if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out hit, 10))
            {
                Chunk hitc;
                if (!StaticWorld.chunks.TryGetValue(hit.collider.gameObject.transform.position, out hitc)) return;

                Vector3 hitBlock;
                if (Input.GetMouseButtonDown(0))
                {
                    hitBlock = hit.point - hit.normal / 2.0f;

                }
                else
                    hitBlock = hit.point + hit.normal / 2.0f;

                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                bool update = false;
                if (Input.GetMouseButtonDown(0))
                {
                    //update = hitc.chunkData[x, y, z].HitBlock();

                    Block block = StaticWorld.GetWorldBlock(hitBlock);

                    block.SetType(BlockType.AIR);

                    hitc.ReDrawChunk();

                    update = true;
                }
                else
                {
                    Block block = StaticWorld.GetWorldBlock(hitBlock);
                    block.SetType(BlockType.STONE);

                    hitc.ReDrawChunk();

                    update = true;

                }

                if (update)
                {

                    List<Vector3> updates = new List<Vector3>();
                    float thisChunkx = hitc.chunk.transform.position.x;
                    float thisChunky = hitc.chunk.transform.position.y;
                    float thisChunkz = hitc.chunk.transform.position.z;

                    //updates.Add(hit.collider.gameObject.name);

                    //update neighbours?
                    if (x == 0)
                        updates.Add(new Vector3(thisChunkx - World.chunkSize, thisChunky, thisChunkz));
                    if (x == World.chunkSize - 1)
                        updates.Add(new Vector3(thisChunkx + World.chunkSize, thisChunky, thisChunkz));
                    if (y == 0)
                        updates.Add(new Vector3(thisChunkx, thisChunky - World.chunkSize, thisChunkz));
                    if (y == World.chunkSize - 1)
                        updates.Add(new Vector3(thisChunkx, thisChunky + World.chunkSize, thisChunkz));
                    if (z == 0)
                        updates.Add(new Vector3(thisChunkx, thisChunky, thisChunkz - World.chunkSize));
                    if (z == World.chunkSize - 1)
                        updates.Add(new Vector3(thisChunkx, thisChunky, thisChunkz + World.chunkSize));

                    foreach (Vector3 cname in updates)
                    {
                        if (StaticWorld.chunks.TryGetValue(cname, out Chunk c))
                        {
                            c.ReDrawChunk();
                        }
                    }
                }
            }
        }
    }
}


