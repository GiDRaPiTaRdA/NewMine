#if UNITY_EDITOR
using Assets.Standard_Assets.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class PlatformTarget : MonoBehaviour
    {
        public BuildTarget activeTarget;

        public PlatformTarget()
        {
            PlatformUpdate.BuildTargetChangedListeners += this.UpdateBuildTarget;
        }

        void OnDestroy()
        {
            PlatformUpdate.BuildTargetChangedListeners -= this.UpdateBuildTarget;
        }

        void UpdateBuildTarget(BuildTarget currentTarget)
        {
            this.gameObject.SetActive(currentTarget == this.activeTarget);
        }
    }
}
#endif

