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

    [SerializeField] private List<GameObject> candlePuzzleDisplays;
    
    public static event Action<PuzzleType> PuzzleSolved;
    
    
    private void Start()
    {
        SetInitialDisplay();
    }


    public void UpdateCandleCounter(int change, CandleColor color)
    {
        //if (isServer && !candlePuzzleSolved)
        if (isServer)
        {
            if (color == CandleColor.Green)
            {
                greenCandleCounter += change;
                greenCandleDisplay.text = greenCandleCounter.ToString();
                RpcUpdateCandleCounterClient();
            }
            else if (color == CandleColor.Purple)
            {
                purpleCandleCounter += change;
                purpleCandleDisplay.text = purpleCandleCounter.ToString();
                RpcUpdateCandleCounterClient();
            }

            if (purpleCandleCounter == greenCandleCounter)
            {
                candlePuzzleSolved = true;
                PuzzleSolved?.Invoke(PuzzleType.Candle);
                foreach (var image in candlePuzzleDisplays)
                {
                    image.gameObject.SetActive(false);
                }
                candlePuzzleDisplays[0].SetActive(true);
                Debug.Log("The candle Puzzle was solved");
            }
            else
            {
                if (purpleCandleCounter < greenCandleCounter)
                {
                    if ((purpleCandleCounter + 3) < greenCandleCounter)
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[2].SetActive(true);
                    }
                    else
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[1].SetActive(true);
                    }
                }
                else if (purpleCandleCounter > greenCandleCounter)
                {
                    if (purpleCandleCounter > (greenCandleCounter + 3))
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[4].SetActive(true);
                    }
                    else
                    { foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[3].SetActive(true);
                    }
                }
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
    public void RpcUpdateCandleCounterClient()
    {
        if (purpleCandleCounter == greenCandleCounter)
            {
                candlePuzzleSolved = true;
                foreach (var image in candlePuzzleDisplays)
                {
                    image.gameObject.SetActive(false);
                }
                candlePuzzleDisplays[0].SetActive(true);
                Debug.Log("The candle Puzzle was solved");
            }
            else
            {
                if (purpleCandleCounter < greenCandleCounter)
                {
                    if ((purpleCandleCounter + 3) < greenCandleCounter)
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[2].SetActive(true);
                    }
                    else
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[1].SetActive(true);
                    }
                }
                else if (purpleCandleCounter > greenCandleCounter)
                {
                    if (purpleCandleCounter > (greenCandleCounter + 3))
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[4].SetActive(true);
                    }
                    else
                    { foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }
                        candlePuzzleDisplays[3].SetActive(true);
                    }
                }
            }
        
        // displayTextObject.text = newValue.ToString();
        /*
        if (color == CandleColor.Green)
        {
            greenCandleDisplay.text = greenCandleCounter.ToString();
        }
        else if (color == CandleColor.Purple)
        {
            purpleCandleDisplay.text = purpleCandleCounter.ToString();
        }
        
        
        if (purpleCandleCounter == greenCandleCounter)
        {
            foreach (var image in candlePuzzleDisplays)
            {
                image.gameObject.SetActive(false);
            }
            candlePuzzleDisplays[0].SetActive(true);
            
        }
        else
        {
            if (purpleCandleCounter < greenCandleCounter)
            {
                foreach (var image in candlePuzzleDisplays)
                {
                    image.gameObject.SetActive(false);
                }
                candlePuzzleDisplays[1].SetActive(true);
            }
            else if (purpleCandleCounter > greenCandleCounter)
            {
                foreach (var image in candlePuzzleDisplays)
                {
                    image.gameObject.SetActive(false);
                }
                candlePuzzleDisplays[3].SetActive(true);
            }
        }
        */
    }


    public void SetInitialDisplay()
    {

        if (isServer)
        {


            if (purpleCandleCounter == greenCandleCounter)
            {
                candlePuzzleSolved = true;
                PuzzleSolved?.Invoke(PuzzleType.Candle);
                foreach (var image in candlePuzzleDisplays)
                {
                    image.gameObject.SetActive(false);
                }

                candlePuzzleDisplays[0].SetActive(true);
                Debug.Log("The candle Puzzle was solved");
            }
            else
            {
                if (purpleCandleCounter < greenCandleCounter)
                {
                    if ((purpleCandleCounter + 3) < greenCandleCounter)
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }

                        candlePuzzleDisplays[2].SetActive(true);
                    }
                    else
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }

                        candlePuzzleDisplays[1].SetActive(true);
                    }
                }
                else if (purpleCandleCounter > greenCandleCounter)
                {
                    if (purpleCandleCounter > (greenCandleCounter + 3))
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }

                        candlePuzzleDisplays[4].SetActive(true);
                    }
                    else
                    {
                        foreach (var image in candlePuzzleDisplays)
                        {
                            image.gameObject.SetActive(false);
                        }

                        candlePuzzleDisplays[3].SetActive(true);
                    }
                }

                candlePuzzleSolved = false;
                Debug.Log("The candles are still not equal");
            }
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