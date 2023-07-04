using UnityEngine;
using Mirror;
using TMPro;

public class PanelInput : NetworkBehaviour
{

    [SerializeField] 
    private TMP_Text displayTextObject;

    [SerializeField] [SyncVar] 
    private string currentLetter;
    
    [Command(requiresAuthority = false)]
    public void EnterLetter(string letter)
    {
        Debug.Log("Switch the last entered Letter");
        currentLetter = letter;
        displayTextObject.text = letter;
        SwitchDisplayedLetter(currentLetter);
    }

    [ClientRpc]
    public void SwitchDisplayedLetter(string letter)
    {
        displayTextObject.text = letter;
        Debug.Log("ClientRpc should have been executed.");
    } 
}
