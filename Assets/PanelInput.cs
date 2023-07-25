using System;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PanelInput : NetworkBehaviour
{
    public static event Action<PuzzleType> PuzzleSolved;

    [SyncVar][SerializeField]
    private bool codePuzzleSolved;
    
    [SerializeField] 
    private TMP_Text displayTextObject;
    
    [SerializeField] 
    private Image statusLed;

    [SerializeField] [SyncVar] 
    private string currentLetter;
    
    [Command(requiresAuthority = false)]
    public void EnterLetter(string letter)
    {
        if (codePuzzleSolved == false)
        {
            Debug.Log("Switch the last entered Letter");
            currentLetter += letter;
            displayTextObject.text = currentLetter;
            SwitchDisplayedLetter(currentLetter);
        }
    }

    [ClientRpc]
    public void SwitchDisplayedLetter(string letter)
    {
        displayTextObject.text = letter;
        Debug.Log("ClientRpc should have been executed.");
    }
    
    [ClientRpc]
    public void SwitchLedColor()
    {
        statusLed.color = Color.green;
    }

    private void Update()
    {
        if (isServer && !codePuzzleSolved)
        {
            if (currentLetter == "ԆϿΨ҂¿Ѧ" )
            {
                statusLed.color = Color.green;
                SwitchLedColor();
                codePuzzleSolved = true;
                PuzzleSolved?.Invoke(PuzzleType.Code);
            }
            else if (currentLetter.Length == 6)
            {
                currentLetter = "";
                SwitchDisplayedLetter(currentLetter);
            }
        }
    }
}
