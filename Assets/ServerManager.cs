using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    NetworkManager manager;
    [SerializeField]private GameObject serverCamera;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
        serverCamera.SetActive(GameObject.FindGameObjectWithTag("Player") == null);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 100), "Start Server"))
        {
            manager.StartServer();
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 100, 200, 100), "Stop Server"))
        {
            manager.StopServer();
        }

        if (NetworkServer.active)
        { 
            GUI.Label(new Rect(10, 10, Screen.width, 50), "Status: online \n" + Transport.active);
        }
        else
        {
            GUI.Label(new Rect(10, 10, Screen.width, 50), "Status: offline");
        }
    }
}