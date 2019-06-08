using System;
using System.Collections;
using System.Collections.Generic;
using AR;
using Assets.Scripts;
using GoogleARCore;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ArUiScript : MonoBehaviour
{
    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>

    public GameObject world;

    public int rotationAngle = 0;

    private Vector3 rotationOrigin;

    public Camera camera;

    #region Constants
    private float hitLength = 1000f;
    private static float initialShift = 0;
    #endregion

    public ARController aRcontroller;

    TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                                      TrackableHitFlags.FeaturePointWithSurfaceNormal |
                                      TrackableHitFlags.FeaturePoint |
                                      TrackableHitFlags.PlaneWithinPolygon |
                                      TrackableHitFlags.None |
                                      TrackableHitFlags.PlaneWithinInfinity;



    public void SpawnWorld()
    {
        // get position of center of the sreen
        var cameraPos = GetCenterOfScreenPosition();

        Vector3 pos = Vector3.zero;

        // Physics //////////////////////////////////////////
        //Ray ray = Camera.current.ScreenPointToRay(cameraPos);

        if (Frame.Raycast(cameraPos.x, cameraPos.y, raycastFilter, out TrackableHit hit))
        {
            pos = hit.Pose.position;
        }
        /////////////////////////////////////////////////////

        // Spawn world
        if (pos != Vector3.zero)
        {
            // StaticWorld.Instance.worldSize* World.chunkSize* StaticWorld.K;

            float worldSize = StaticWorld.Instance.worldSize * World.chunkSize * StaticWorld.K;

            Vector3 a = new Vector3(worldSize / 2f, 0f, worldSize / 2f);

            this.world.transform.position = pos-a;

            //this.world.transform.rotation = Quaternion.LookRotation(new Vector3(0,this.camera.transform.rotation.y,0), Vector3.up);
        }
    }

    public void Rotate()
    {
        float worldSize = StaticWorld.Instance.worldSize * World.chunkSize * StaticWorld.K;

        Vector3 a = this.world.transform.rotation * new Vector3(worldSize / 2f, 0f, worldSize / 2f);

        

        this.world.transform.RotateAround(this.world.transform.position + a, new Vector3(0, 1, 0),  this.rotationAngle);
        //this.world.transform.RotateAround(this.world.transform.position + new Vector3(worldSize / 2f, worldSize / 2f, 0f), Vector3.forward, rotationAngle);
        //this.world.transform.Rotate(0, this.rotationAngle,0);
    }

    public static Vector3 GetCenterOfScreenPosition()
    {
        var cameraPos = Camera.current.transform.position;
        cameraPos.x += (int)(Screen.width / 2);
        cameraPos.y += (int)(Screen.height / 2);

        return cameraPos;
    }
}
