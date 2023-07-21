using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class CandleController : NetworkBehaviour
{

    public static event Action<int, CandleColor> CandleSwitched;
    
    [SerializeField] 
    private ParticleSystem candleFlame;

    [SyncVar]
    public bool candleLit;

    [SyncVar] 
    public CandleColor color;

    [SerializeField]
    private Color candleGreen;
    
    [SerializeField]
    private  Color candlePurple;

    /// <summary>
    /// Awake sets the candle flame color based on the enum value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void Awake()
    {
        switch (color)
        {
            case CandleColor.Green:
            {
                var main = candleFlame.main;
                main.startColor = candleGreen;
                break;
            }
            case CandleColor.Purple:
            {
                var main = candleFlame.main;
                main.startColor = candlePurple;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!candleLit)
        {
            candleFlame.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Switches the candle flame on or of at the server.
    /// </summary>
    [Command (requiresAuthority = false)]
    public void CmdSwitchCandleState()
    {
        if (!candleLit)
        {
            //candleFlame.Play();
            candleFlame.gameObject.SetActive(true);
            RpcSwitchCandleState(true);
            candleLit = true;
            CandleSwitched?.Invoke(1, color);
        }
        else if (candleLit)
        {
            //candleFlame.Stop();
            candleFlame.gameObject.SetActive(false);
            RpcSwitchCandleState(false);
            candleLit = false;
            CandleSwitched?.Invoke(-1, color);
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
