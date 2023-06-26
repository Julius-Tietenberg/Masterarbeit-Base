using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnBox : NetworkBehaviour
{

    public GameObject cube;
    
    [Command (requiresAuthority = false)]
    public void CmdSpawnBox()
    { 
        GameObject cubeClone = Instantiate(cube, transform.position, transform.rotation);
        NetworkServer.Spawn(cubeClone);
        // SpawnBoxLocal();
    }
    
    [ClientRpc]
    public void SpawnBoxLocal()
    {
        GameObject cubeClone = Instantiate(cube, transform.position, transform.rotation);
        //NetworkServer.Spawn(cubeClone);
        Debug.Log("Log within the SpawnBox RPC");
    }
}
