using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class CandleController : NetworkBehaviour
{

    public static event Action<int, CandleColor> CandleSwitched;
    
    public AudioSource audioSource;
    public AudioClip candleOff;
    public AudioClip candleOn;
    public float volume;
    
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

    private bool puzzleActive;

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
        if (puzzleActive)
        {
          if (!candleLit)
          {
              //candleFlame.Play();
              candleFlame.gameObject.SetActive(true);
              audioSource.PlayOneShot(candleOn, volume);
              RpcPlayCandleOnAudio();
              RpcSwitchCandleState(true);
              candleLit = true;
              CandleSwitched?.Invoke(1, color);
          }
          else if (candleLit)
          {
              //candleFlame.Stop();
              candleFlame.gameObject.SetActive(false);
              audioSource.PlayOneShot(candleOff, volume);
              RpcPlayCandleOffAudio();
              RpcSwitchCandleState(false);
              candleLit = false;
              CandleSwitched?.Invoke(-1, color);
            }
          Debug.Log("Changed candle state and invoked 'candleSwitched' action");  
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