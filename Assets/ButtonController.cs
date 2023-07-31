using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class ButtonController : NetworkBehaviour
{
   
    // This is invoked to inform all listeners, when a specific button is pressed.
    public static event Action<int> ButtonWasPressed;
    
    public AudioSource audioSource;
    public AudioClip error;
    public AudioClip buttonPress;
    public float volume;

    public GameObject buttonKnob;

    public List<Material> buttonMaterials;

    [SyncVar]
    public bool buttonPressed;
    
    [SyncVar]
    public int buttonSequenceNumber;
    
    [SyncVar]
    public bool puzzleActive;


    [Command (requiresAuthority = false)]
    public void CmdPressButton()
    {
        Debug.Log("CmdPressButton wird gestartet");
        if (!buttonPressed && puzzleActive)
        {
            //play animation (to-do)
            buttonPressed = true;
            audioSource.PlayOneShot(buttonPress, volume);
            RpcPlayButtonAnimationAndAudio();
            ButtonWasPressed?.Invoke(buttonSequenceNumber);
            Debug.Log("Action invoked");
        }
        else
        {
            //some kind of feedback (to-do)
            //buttonPressed = false;
        }
    }

    public void FeedbackButtonCorrect()
    {
        buttonKnob.GetComponent<MeshRenderer>().material = buttonMaterials[1];
        RpcSetButtonClient(1);
    }
    
    public void FeedbackButtonFalse()
    {
        buttonKnob.GetComponent<MeshRenderer>().material = buttonMaterials[2];
        audioSource.PlayOneShot(error, volume);
        RpcPlayErrorAudio();
        RpcSetButtonClient(2);
    }
    
    [ClientRpc]
    public void RpcSetButtonClient(int status)
    {
        buttonKnob.GetComponent<MeshRenderer>().material = buttonMaterials[status];
    }
    
    public void ResetButtonState()
    {
        buttonPressed = false;
        buttonKnob.GetComponent<MeshRenderer>().material = buttonMaterials[0];
        RpcSetButtonClient(0);
    }

    [ClientRpc]
    public void RpcPlayButtonAnimationAndAudio()
    {
        //play animation for client
        audioSource.PlayOneShot(buttonPress, volume);
    }
    
    [ClientRpc]
    public void RpcPlayErrorAudio()
    { 
        audioSource.PlayOneShot(error, volume);
    }
    

    public void SetPuzzleActive()
    {
        puzzleActive = true;
    }
    
    
    private void OnEnable()
    {
        GameFlowManager.StartGame += SetPuzzleActive;
    }

    private void OnDisable()
    {
        GameFlowManager.StartGame -= SetPuzzleActive;
    }
    
}
