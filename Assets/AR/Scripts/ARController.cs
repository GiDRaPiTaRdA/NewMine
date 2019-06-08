//-----------------------------------------------------------------------
// <copyright file="ARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;
using GoogleARCoreInternal;
using UnityStandardAssets.Utility;
using InstantPreviewInput = GoogleARCore.InstantPreviewInput;

namespace AR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.Rendering;

#if UNITY_EDITOR
    using Input = InstantPreviewInput;
#endif

    public class ARController : MonoBehaviour
    {
     

        #region Variables
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject TrackedPlanePrefab;

        public ARCoreSession session;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;


        #endregion

        public bool NotTraking => this.m_AllPlanes.All(p => p.TrackingState != TrackingState.Tracking);

        //public bool PlanesSearch
        //{
        //    get { return this.planesSearch; }
        //    set
        //    {
        //        this.planesSearch = value;
        //        this.SearchingForPlaneUI.SetActive(this.planesSearch);
        //    }
        //}

        public List<GameObject> planes = new List<GameObject>();
        //private bool planesSearch = true;

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            this._QuitOnConnectionErrors();

            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
                if (!this.m_IsQuitting && Session.Status.IsValid()) 
                {
                    this.SearchingForPlaneUI.SetActive(true);
                }

                return;
            }

            // Screen actiive
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Update new planes
            Session.GetTrackables<TrackedPlane>(this.m_NewPlanes, TrackableQueryFilter.New);

            // Update traked planes
            Session.GetTrackables<TrackedPlane>(this.m_AllPlanes);

            if (this.session.SessionConfig.EnablePlaneFinding)
            {
                // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
                for (int i = 0; i < this.m_NewPlanes.Count; i++)
                {
                    GameObject planeObject = Instantiate(this.TrackedPlanePrefab, Vector3.zero, Quaternion.identity,
                        this.transform);
                    this.planes.Add(planeObject);
                    planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(this.m_NewPlanes[i]);
                }
            }
            
            this.SearchingForPlaneUI.SetActive(this.NotTraking&& this.session.SessionConfig.EnablePlaneFinding);
        }


      

        #region Other

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            if (this.m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                this._ShowAndroidToastMessage("Camera permission is needed to run this application.");
                this.m_IsQuitting = true;
                this.Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                this._ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                this.m_IsQuitting = true;
                this.Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
        #endregion
    }
}
