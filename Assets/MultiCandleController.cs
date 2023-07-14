using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MultiCandleController : NetworkBehaviour
{
    public static event Action<int, CandleColor> CandlesSwitched;
    
    [SerializeField] 
    private List<ParticleSystem> candleFlames;

    [SyncVar]
    public bool candlesLit;
    
    [SyncVar]
    public int amountOfCandles;
    
    [SyncVar] 
    public CandleColor color;

    [SerializeField]
    private Color candleGreen;
    
    [SerializeField]
    private  Color candlePurple;
    
    private void Awake()
    {
        switch (color)
        {
            case CandleColor.Green:
            {
                foreach (var flame in candleFlames)
                {
                    var main = flame.main;
                    main.startColor = candleGreen;
                }
                break;
            }
            case CandleColor.Purple:
            {
                foreach (var flame in candleFlames)
                {
                    var main = flame.main;
                    main.startColor = candlePurple;
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// Switches the candle flame on or of at the server.
    /// </summary>
    [Command (requiresAuthority = false)]
    public void CmdSwitchCandleState()
    {
        if (!candlesLit)
        {
            foreach (var flame in candleFlames)
            {
                flame.gameObject.SetActive(true);
            }
            RpcSwitchCandleState(true);
            candlesLit = true;
            CandlesSwitched?.Invoke(amountOfCandles, color);
        }
        else if (candlesLit)
        {
            foreach (var flame in candleFlames)
            {
                flame.gameObject.SetActive(false);
            }
            RpcSwitchCandleState(false);
            candlesLit = false;
            CandlesSwitched?.Invoke(-amountOfCandles, color);
        }
        Debug.Log("Changed candles state and invoked 'candleSwitched' action");
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
            foreach (var flame in candleFlames)
            {
                flame.gameObject.SetActive(true);
            }
        }
        else if (!candleLitClient)
        {
            foreach (var flame in candleFlames)
            {
                flame.gameObject.SetActive(false);
            }
        }
        Debug.Log("Candles RPC");
    }
}
