using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MaterialSwitcher : NetworkBehaviour
{
    
    [SerializeField] private Material[] cubeMat;
    
    [Command(requiresAuthority = false)]
    public void SwitchMaterial()
    {
        Debug.Log("Switch the material now");
        MatExchange();
        Debug.Log("ClientRpc should have been executed.");
    }

    [ClientRpc]
    public void MatExchange()
    {
        if (gameObject.GetComponent<MeshRenderer>().material == cubeMat[0])
        {
            gameObject.GetComponent<MeshRenderer>().material = cubeMat[1];
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = cubeMat[0];
        }
    }
    
    [Command(requiresAuthority = false)]
    public void ChangePosition()
    {
        Debug.Log("Switch the material now");
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        PosChange();
        Debug.Log("ClientRpc should have been executed.");
    }

    [ClientRpc]
    public void PosChange()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Debug.Log("Log within the RPC");
    }
}
