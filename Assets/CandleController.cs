using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class CandleController : NetworkBehaviour
{

    // This is invoked to inform all listeners, when the candles state changes.
    public static event Action<int> CandleSwitched;
    
    [SerializeField] 
    private ParticleSystem candleFlame;

    [SyncVar]
    public bool candleLit;

    
    /// <summary>
    /// Switches the candle flame on or of at the server.
    /// </summary>
    [Command (requiresAuthority = false)]
    public void CmdSwitchCandleState()
    {
        Debug.Log("CandleSwitchCmd wird gestartet");
        if (!candleLit)
        {
            //candleFlame.Play();
            candleFlame.gameObject.SetActive(true);
            RpcSwitchCandleState(true);
            candleLit = true;
            CandleSwitched?.Invoke(+1);
        }
        else if (candleLit)
        {
            //candleFlame.Stop();
            candleFlame.gameObject.SetActive(false);
            RpcSwitchCandleState(false);
            candleLit = false;
            CandleSwitched?.Invoke(-1);
        }
        Debug.Log("Changed candle state and invoked 'candleSwitched' action");
    }
    
    
    /// <summary>
    /// Syncs the server candle state with all clients.
    /// </summary>
    /// <param name="candleLitClient"></param>
    [ClientRpc]
    public void RpcSwitchCandleState(bool candleLitClient)
    {
        if (candleLitClient)
        {
            //candleFlame.Play();
            candleFlame.gameObject.SetActive(true);
        }
        else if (!candleLitClient)
        {
            //candleFlame.Stop();
            candleFlame.gameObject.SetActive(false);
        }
        Debug.Log("Candle RPC");
    }
}
