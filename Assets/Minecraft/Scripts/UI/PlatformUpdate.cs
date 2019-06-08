//#define PLATFORM_UPDATE_DEBUG

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Assets.Standard_Assets.Editor
{
    [ExecuteInEditMode]
    public class PlatformUpdate : MonoBehaviour, IActiveBuildTargetChanged
    {
        public static System.Action<BuildTarget> BuildTargetChangedListeners;

        BuildTarget currentTarget;
        public int callbackOrder { get { return 0; } }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            this.currentTarget = newTarget;
            // Can't update immediately as scripts aren't ready
            EditorApplication.update += this.UpdateGameObjects;

#if PLATFORM_UPDATE_DEBUG
        Debug.Log("Switched build target to " + currentTarget);
#endif
        }

        void UpdateGameObjects()
        {
            if (BuildTargetChangedListeners != null)
                BuildTargetChangedListeners(this.currentTarget);
            EditorApplication.update -= this.UpdateGameObjects;

#if PLATFORM_UPDATE_DEBUG
        Debug.Log("Updated listeners with new target: " + currentTarget);
#endif
        }
    }
}
#endif