using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public enum CandleColor
{
    Purple,
    Green
}

public class CandlePuzzleControler : NetworkBehaviour
{
    [SerializeField] 
    private TMP_Text displayTextObject;
    
    [SerializeField] 
    private TMP_Text greenCandleDisplay;
    
    [SerializeField] 
    private TMP_Text purpleCandleDisplay;
    
    [SyncVar] 
    public int candleCounter;
    
    [SyncVar] 
    public int greenCandleCounter;
    
    [SyncVar] 
    public int purpleCandleCounter;
    
    [SyncVar] 
    public bool candlePuzzleSolved;
    
    
    public void UpdateCandleCounter(int change, CandleColor color)
    {
        //if (isServer && !candlePuzzleSolved)
        if (isServer)
        {
            if (color == CandleColor.Green)
            {
                greenCandleCounter += change;
                greenCandleDisplay.text = greenCandleCounter.ToString();
                RpcUpdateCandleCounterClient(greenCandleCounter, CandleColor.Green);
            }
            else if (color == CandleColor.Purple)
            {
                purpleCandleCounter += change;
                purpleCandleDisplay.text = purpleCandleCounter.ToString();
                RpcUpdateCandleCounterClient(purpleCandleCounter, CandleColor.Purple);
            }

            if (purpleCandleCounter == greenCandleCounter)
            {
                candlePuzzleSolved = true;
                Debug.Log("The candle Puzzle was solved");
            }
            else
            {
                candlePuzzleSolved = false;
                Debug.Log("The candles are still not equal");
            }
            
            /*
            candleCounter += change;
            displayTextObject.text = candleCounter.ToString();
            RpcUpdateCandleCounterClient(candleCounter);
            */
        }
    }

    [ClientRpc]
    public void RpcUpdateCandleCounterClient(int newValue, CandleColor color)
    {
        // displayTextObject.text = newValue.ToString();
        
        if (color == CandleColor.Green)
        {
            greenCandleDisplay.text = greenCandleCounter.ToString();
        }
        else if (color == CandleColor.Purple)
        {
            purpleCandleDisplay.text = purpleCandleCounter.ToString();
        }
    }

    private void OnEnable()
    {
        CandleController.CandleSwitched += UpdateCandleCounter;
        MultiCandleController.CandlesSwitched += UpdateCandleCounter;
    }

    private void OnDisable()
    {
        CandleController.CandleSwitched -= UpdateCandleCounter;
        MultiCandleController.CandlesSwitched -= UpdateCandleCounter;
    }
}