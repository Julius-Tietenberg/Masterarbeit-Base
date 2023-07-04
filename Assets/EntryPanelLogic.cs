using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class EntryPanelLogic : NetworkBehaviour
{
    // The solution sequence as integers.
    [SerializeField] private readonly SyncList<int> correctSequence = new SyncList<int>() {3, 7, 5, 9, 4};

    // The current player entered sequence.
    [SerializeField] private readonly SyncList<int> currentlyEnteredSequence = new SyncList<int>();
    
    
    [SerializeField] private List<GameObject> DisplayFieldsUnSynced = new List<GameObject>();
    [SerializeField] private readonly SyncList<GameObject> DisplayFields = new SyncList<GameObject>();

    // Is the puzzle solved?
    [SerializeField] [SyncVar] private bool sequenzePuzzleSolved;
    
    [SerializeField] private Material solvedMat;
    [SerializeField] [SyncVar] private GameObject solvedSphere;

    private void Awake()
    {
        foreach (var display in DisplayFieldsUnSynced)
        {
            DisplayFields.Add(display);
        }
        
        // correctSequence.Add(3);
        // correctSequence.Add(7);
        // correctSequence.Add(5);
        // correctSequence.Add(9);
        // correctSequence.Add(4);
    }
    
    private void Update()
    {
        CheckEnteredSequence();
    }
    
    

    [ClientRpc]
    public void UpdateSequenceDisplay()
    {
        var counter = 0;
        foreach (var display in DisplayFields)
        {
            var textField = display.GetComponentInChildren<TMP_Text>();
            textField.text = currentlyEnteredSequence[counter].ToString();
            counter++;
        }
    }

    [ClientRpc]
    public void SwitchMat()
    {
        solvedSphere.GetComponent<MeshRenderer>().material = solvedMat;
    }
    

    [Command]
    public void CmdEnterNewSymbol(int entry)
    {
        if (currentlyEnteredSequence.Count < 5 && entry >= 1 && entry <= 10)
        {
            currentlyEnteredSequence.Add(entry);
            UpdateSequenceDisplay();
        }
        Debug.Log("CmdEnterNewSymbol was run!");
    }

    /// <summary>
    /// Check if the entered sequence is correct.
    ///  -> If yes, the puzzle is solved.
    ///  -> If no, reset the entry panel.
    /// </summary>
    private void CheckEnteredSequence()
    {
        if (currentlyEnteredSequence.Count == 5 && sequenzePuzzleSolved == false)
        {
            if (currentlyEnteredSequence == correctSequence)
            {
                // Correct Logic
                sequenzePuzzleSolved = true;
                SwitchMat();
            }
            else
            {
                //False Logic
                // Some feedback (coroutine?) for the players
                currentlyEnteredSequence.Clear();
            }
        }  
    }
}