using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MultiCandleController : NetworkBehaviour
{
    public static event Action<int, CandleColor> CandlesSwitched;
    
    public AudioSource audioSource;
    public AudioClip candleOff;
    public AudioClip candleOn;
    public float volume;
    
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

    private bool puzzleActive;
    
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
        
        if (!candlesLit)
        {
            foreach (var flame in candleFlames)
            {
                flame.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Switches the candle flame on or of at the server.
    /// </summary>
    [Command (requiresAuthority = false)]
    public void CmdSwitchCandleState()
    {
        if (puzzleActive)
        {
         if (!candlesLit)
         {
             audioSource.PlayOneShot(candleOn, volume);
             RpcPlayCandleOnAudio();
             foreach (var flame in candleFlames)
             {
                 flame.gameObject.SetActive(true);
             } 
             RpcSwitchCandleState(true);
             candlesLit = true; CandlesSwitched?.Invoke(amountOfCandles, color);
         }
         else if (candlesLit) 
         { 
             audioSource.PlayOneShot(candleOff, volume);
             RpcPlayCandleOffAudio();
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
    
    public void SetPuzzleActive()
    {
        puzzleActive = true;
    }
    
    public void PuzzleInactive()
    {
        puzzleActive = false;
    }
    
    [ClientRpc]
    public void RpcPlayCandleOffAudio()
    { 
        audioSource.PlayOneShot(candleOff, volume);
    }
    
    [ClientRpc]
    public void RpcPlayCandleOnAudio()
    { 
        audioSource.PlayOneShot(candleOn, volume);
    }
    
    private void OnEnable()
    {
        GameFlowManager.StartGame += SetPuzzleActive;
        GameFlowManager.EndCandlePuzzle += PuzzleInactive;
    }

    private void OnDisable()
    {
        GameFlowManager.StartGame -= SetPuzzleActive;
        GameFlowManager.EndCandlePuzzle -= PuzzleInactive;
    }
}
