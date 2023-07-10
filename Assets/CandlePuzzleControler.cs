using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CandlePuzzleControler : NetworkBehaviour
{
    [SerializeField] 
    private TMP_Text displayTextObject;
    
    [SyncVar] 
    public int candleCounter;
    
    public void UpdateCandleCounter(int change)
    {
        if (isServer)
        {
            candleCounter += change;
            displayTextObject.text = candleCounter.ToString();
            RpcUpdateCandleCounterClient(candleCounter);
        }
    }

    [ClientRpc]
    public void RpcUpdateCandleCounterClient(int newValue)
    {
        displayTextObject.text = newValue.ToString();
    }

    private void OnEnable()
    {
        CandleController.CandleSwitched += UpdateCandleCounter;
    }

    private void OnDisable()
    {
        CandleController.CandleSwitched -= UpdateCandleCounter;
    }
}