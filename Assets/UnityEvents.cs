using UnityEngine.Events;

namespace Assets
{
    [System.Serializable]
    public class EventInt : UnityEvent<int> { }

    [System.Serializable]
    public class EventBool : UnityEvent<bool> { }

    [System.Serializable]
    public class EventString : UnityEvent<string> { }
}