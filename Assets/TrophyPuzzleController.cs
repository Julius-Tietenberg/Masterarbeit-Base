using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TrophyPuzzleController : NetworkBehaviour
{

    public static event Action<PuzzleType> PuzzleSolved;

    [SerializeField] private Animator lockAnimator;
    
    [SerializeField] public Vector3 positionOne;
    [SerializeField] public Vector3 positionTwo;
    [SerializeField] public Vector3 positionThree;
    [SerializeField] public Vector3 positionFour;

    [SerializeField] public List<Vector3> trophyPositions; 

    [SerializeField] private List<GameObject> solutionOrder;

    [SerializeField] private List<GameObject> currentOrder;

    [SyncVar] public bool TrophyPuzzleSolved;
    [SyncVar] public bool puzzleActive;

    [SerializeField] private GameObject buttonCanvas;

    private void Awake()
    {
        trophyPositions[0] = currentOrder[0].transform.position;
        trophyPositions[1] = currentOrder[1].transform.position;
        trophyPositions[2] = currentOrder[2].transform.position;
        trophyPositions[3] = currentOrder[3].transform.position;
    }
    
    [Command (requiresAuthority = false)]
    public void CmdSwitchTrophyPositions(int buttonNr)
    {
        if (TrophyPuzzleSolved != true && puzzleActive)
        {
            if (buttonNr == 1)
            {
                var leftTrophy = currentOrder[0]; 
                var rightTrophy = currentOrder[1];
                currentOrder[0] = rightTrophy;
                currentOrder[1] = leftTrophy;
            }
            else if (buttonNr == 2)
            {
                var leftTrophy = currentOrder[1];
                var rightTrophy = currentOrder[2];
                currentOrder[1] = rightTrophy;
                currentOrder[2] = leftTrophy;
            }
            else if (buttonNr == 3)
            {
                var leftTrophy = currentOrder[2]; 
                var rightTrophy = currentOrder[3];
                currentOrder[2] = rightTrophy;
                currentOrder[3] = leftTrophy;
            }
            var counter = 0;
            foreach (var trophy in currentOrder)
            {
                trophy.transform.position = trophyPositions[counter];
                counter++;
            }
            
            RpcSwitchTrophyPositions(buttonNr);
            
        }
        
        if (currentOrder[0] == solutionOrder[0] && currentOrder[1] == solutionOrder[1] && currentOrder[2] == solutionOrder[2] && currentOrder[3] == solutionOrder[3])
        {
            TrophyPuzzleSolved = true;
            PuzzleSolved?.Invoke(PuzzleType.Trophy);
            buttonCanvas.SetActive(false);
            // Give some additional feedback
            lockAnimator.SetTrigger("isLockOpen");
            RpcSolvedFeedback();
        }
    }

    [ClientRpc]
    public void RpcSwitchTrophyPositions(int buttonNr)
    {
        if (buttonNr == 1)
        {
            var leftTrophy = currentOrder[0]; 
            var rightTrophy = currentOrder[1];
            currentOrder[0] = rightTrophy;
            currentOrder[1] = leftTrophy;
        }
        else if (buttonNr == 2)
        {
            var leftTrophy = currentOrder[1];
            var rightTrophy = currentOrder[2];
            currentOrder[1] = rightTrophy;
            currentOrder[2] = leftTrophy;
        }
        else if (buttonNr == 3)
        {
            var leftTrophy = currentOrder[2]; 
            var rightTrophy = currentOrder[3];
            currentOrder[2] = rightTrophy;
            currentOrder[3] = leftTrophy;
        }
        var counter = 0;
        foreach (var trophy in currentOrder)
        {
            trophy.transform.position = trophyPositions[counter];
            counter++;
        }
    }
    
    public void SetPuzzleActive()
    {
        puzzleActive = true;
    }

    [ClientRpc]
    public void RpcSolvedFeedback()
    {
        buttonCanvas.SetActive(false);
        lockAnimator.SetTrigger("isLockOpen");
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

