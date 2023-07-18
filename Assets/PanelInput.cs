using System;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PanelInput : NetworkBehaviour
{

    [SerializeField] 
    private TMP_Text displayTextObject;
    
    [SerializeField] 
    private Image statusLed;

    [SerializeField] [SyncVar] 
    private string currentLetter;
    
    [Command(requiresAuthority = false)]
    public void EnterLetter(string letter)
    {
        Debug.Log("Switch the last entered Letter");
        currentLetter += letter;
        displayTextObject.text = currentLetter;
        SwitchDisplayedLetter(currentLetter);
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
        if (isServer)
        {
            if (currentLetter == "ԆϿΨ҂¿Ѧ" )
            {
                statusLed.color = Color.green;
                SwitchLedColor();
            }
            else if (currentLetter.Length == 6)
            {
                currentLetter = "";
                SwitchDisplayedLetter(currentLetter);
            }
        }
    }
}
