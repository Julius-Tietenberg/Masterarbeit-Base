using System.Collections;
using System.Collections.Generic;
using Mirror;
// using Player;
using UnityEngine;
using UnityEngine.Events;

namespace EventHelper
{
    public class EventHelper : MonoBehaviour
    {
    }

    [System.Serializable]
    public class UnityIntEvent : UnityEvent<int>
    {
    }

    public class UnityNetworkConnectionEvent : UnityEvent<NetworkConnection>
    {
    }
    /*
    public class UnityPlayerDataEvent : UnityEvent<PlayerData>
    {
    }
    */
    public class UnityQuaternionArrayEvent : UnityEvent<Quaternion[]>
    {
    }
}
