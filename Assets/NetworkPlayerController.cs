using System;
using System.Collections.Generic;
using EventHelper;
using Mirror;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    /*
    [SerializeField] private float fingerRotationInterval = 0.1F;
    private float fingerRotationsCooldown;

    [SerializeField] private PlayerData _playerData;
    private UnityQuaternionArrayEvent _onServerRecieveLeftFingerRotationUpdate = new UnityQuaternionArrayEvent();
    private UnityQuaternionArrayEvent _onServerRecieveRightFingerRotationUpdate = new UnityQuaternionArrayEvent();

    public UnityQuaternionArrayEvent OnServerRecieveLeftFingerRotationUpdate { get => _onServerRecieveLeftFingerRotationUpdate; set => _onServerRecieveLeftFingerRotationUpdate = value; }
    public UnityQuaternionArrayEvent OnServerRecieveRightFingerRotationUpdate { get => _onServerRecieveRightFingerRotationUpdate; set => _onServerRecieveRightFingerRotationUpdate = value; }

    private UnityQuaternionArrayEvent _onClientLeftFingerRotationUpdate = new UnityQuaternionArrayEvent();
    private UnityQuaternionArrayEvent _onClientRightFingerRotationUpdate = new UnityQuaternionArrayEvent();

    public UnityQuaternionArrayEvent OnClientLeftFingerRotationUpdate { get => _onClientLeftFingerRotationUpdate; set => _onClientLeftFingerRotationUpdate = value; }
    public UnityQuaternionArrayEvent OnClientRightFingerRotationUpdate { get => _onClientRightFingerRotationUpdate; set => _onClientRightFingerRotationUpdate = value; }
    */
    
    [SerializeField] private Transform _networkHead, _networkLeftHand, _networkRightHand;
    private Transform _oculusHead, _oculusLeftHand, _oculusRightHand;

    
    [Header("Local Avatar Rendering")]
    [Tooltip("If set to 'true', you can only see the full avatar of your partner.")]
    [SerializeField] private bool deactivateLocalAvatarDisplay;
    // Lists contain the renderes of all visible avatar parts and tools. 
    [SerializeField] private List<SkinnedMeshRenderer> AvatarRenderers;
    [SerializeField] private List<MeshRenderer> ToolRenderers;

    private void Start()
    {
        VrNetworkManager.singleton.OnServerAddPlayerEvent.AddListener(OnServerAddPlayer);

        _networkLeftHand.gameObject.SetActive(!isLocalPlayer);
        _networkRightHand.gameObject.SetActive(!isLocalPlayer);
        /*
        if (!isLocalPlayer)
        {
            _onClientLeftFingerRotationUpdate.AddListener(_networkLeftHand.GetComponent<FingerRotations>().SetInterpolate);
            _onClientRightFingerRotationUpdate.AddListener(_networkRightHand.GetComponent<FingerRotations>().SetInterpolate);
        }
        if (isServer)
        {
            _onServerRecieveLeftFingerRotationUpdate.AddListener(_networkLeftHand.GetComponent<FingerRotations>().SetInterpolate);
            _onServerRecieveRightFingerRotationUpdate.AddListener(_networkRightHand.GetComponent<FingerRotations>().SetInterpolate);
        }
        */
        enabled = isLocalPlayer;
        if (!enabled) return;

        Transform XRCameraRig = GameObject.FindWithTag("Player").transform;

        TransformHolder transformHolder = XRCameraRig.GetComponent<TransformHolder>();

        _oculusHead = transformHolder.Head;
        _oculusLeftHand = transformHolder.LeftHand;
        _oculusRightHand = transformHolder.RightHand;
        
        //ChangeLayer(this.gameObject, 7);
        DeactivateLocalAvatarRendering();
        
    }

    void SetLayerRecursively(GameObject g, int layer)
    {
        g.layer = layer;

        foreach (Transform child in g.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    /// <summary>
    /// This method will deactivate the renderers for all (networked) avatar parts and tools on the local player.
    /// You will still be able to see your (local) tools on the server, while your partner can only see the networked avatar and tools.
    /// </summary>
    public void DeactivateLocalAvatarRendering()
    {
        if (isLocalPlayer && deactivateLocalAvatarDisplay)
        { 
            foreach (var renderer in AvatarRenderers)
            {
                renderer.enabled = false;
            }
            foreach (var renderer in ToolRenderers)
            {
                renderer.enabled = false;
            }
        }
    }
    

    [ClientRpc(includeOwner = false)]
    public void ChangeLayer(GameObject g, int layer)
    {
        g.layer = layer;

        foreach (Transform child in g.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    
    /*
    [Command]
    public void CmdSendPlayerDataToServer(PlayerData playerData)
    {
        _playerData = playerData;
        RpcUpdateNetworkPlayerData(playerData);
        GetComponent<AvatarSpawner>().SpawnAvatar(playerData,_networkHead,_networkLeftHand,_networkRightHand,false);
    }
    */
    private void OnServerAddPlayer(NetworkConnection con)
    {
        // RpcUpdateNetworkPlayerData(_playerData);
        
    }
    /*
    [ClientRpc]
    private void RpcUpdateNetworkPlayerData(PlayerData playerData)
    {
        //_playerData = playerData;
        //GetComponent<AvatarSpawner>().SpawnAvatar(playerData, _networkHead, _networkLeftHand, _networkRightHand, true);
    }
    */
    /*
    [Command]
    private void CmdSetFingerRotations(Quaternion[] quaternions, bool left)
    {
        RpcSetFingerRotations(quaternions, left);
        if (left)
        {
            OnServerRecieveLeftFingerRotationUpdate.Invoke(quaternions);
        }
        else
        {
            OnServerRecieveRightFingerRotationUpdate.Invoke(quaternions);
        }
    }
    */
    /*
    [ClientRpc]
    private void RpcSetFingerRotations(Quaternion[] quaternions, bool left)
    {
        if (!isLocalPlayer)
        {
            if (left) OnClientLeftFingerRotationUpdate.Invoke(quaternions);
            else OnClientRightFingerRotationUpdate.Invoke(quaternions);
        }
    }
    */
    private void Update()
    {

        if (_oculusHead && _networkLeftHand && _oculusRightHand)
        {
            try
            {
                _networkHead.position = _oculusHead.position; 
                _networkHead.rotation = _oculusHead.rotation;
            
                _networkLeftHand.position = _oculusLeftHand.position;
                _networkLeftHand.rotation = _oculusLeftHand.rotation;
            
                _networkRightHand.position = _oculusRightHand.position;
                _networkRightHand.rotation = _oculusRightHand.rotation;
            }
            catch (Exception e)
            {
                Debug.Log("Race condition (Network Player Controller occured!");
                Console.WriteLine(e);
                throw;
            }
        }
        
        
        /*
        if (fingerRotationsCooldown > 0)
        {
            fingerRotationsCooldown -= Time.deltaTime;
        }
        else
        {
            Quaternion[] leftFingerRotations = _oculusLeftHand.GetComponent<FingerRotations>().Get();
            Quaternion[] rightFingerRotations = _oculusRightHand.GetComponent<FingerRotations>().Get();

            CmdSetFingerRotations(leftFingerRotations, true);
            CmdSetFingerRotations(rightFingerRotations, false);

            fingerRotationsCooldown = fingerRotationInterval;
        }
        */
    }
}