using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialCandleController : MonoBehaviour
{
    public static event Action<int> CandleTutorialUsed;
    
    public AudioSource audioSource;
    public AudioClip candleOff;
    public AudioClip candleOn;
    public float volume;
    
    [SerializeField] 
    private List<ParticleSystem> candleFlames;
    
    public bool candlesLit;
    
    [SerializeField]
    private Color candleNeutral;
    
    private void Awake()
    {
        foreach (var flame in candleFlames)
        {
            var main = flame.main;
            main.startColor = candleNeutral;
            flame.gameObject.SetActive(false);
            candlesLit = false;
        }
    }
    
    /// <summary>
    /// Switches the candle flame on or of at the server.
    /// </summary>
    public void SwitchCandleState()
    {
        if (!candlesLit)
        {
            audioSource.PlayOneShot(candleOn, volume);
            foreach (var flame in candleFlames)
            {
                flame.gameObject.SetActive(true);
            }
            candlesLit = true;
        }
        else if (candlesLit) 
        { 
            audioSource.PlayOneShot(candleOff, volume);
            foreach (var flame in candleFlames)
            { 
                flame.gameObject.SetActive(false);
            }
            candlesLit = false;
        }
        CandleTutorialUsed?.Invoke(3);
    }
}
