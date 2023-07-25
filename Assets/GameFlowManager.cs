using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public enum PuzzleType
{
   Candle,
   Code,
   Button,
   Trophy
}

public class GameFlowManager : NetworkBehaviour
{
   
   [SerializeField] private List<Image> puzzleIcons;

   [SerializeField] private Color solvedGreen;

   public bool trophyPuzzleSolved;
   public bool candlePuzzleSolved;
   public bool codePuzzleSolved;
   public bool buttonPuzzleSolved;

   public void OnPuzzleSolved(PuzzleType type)
   {
      if (type == PuzzleType.Code)
      {
         codePuzzleSolved = true;
         HandlePuzzleFinished(0);
      }
      else if (type == PuzzleType.Candle)
      {
         candlePuzzleSolved = true;
         HandlePuzzleFinished(1);
      }
      else if (type == PuzzleType.Trophy)
      {
         trophyPuzzleSolved = true;
         HandlePuzzleFinished(2);
      }
      else if (type == PuzzleType.Button)
      {
         buttonPuzzleSolved = true;
         HandlePuzzleFinished(3);
      }
   }
   
   public void HandlePuzzleFinished(int puzzleNr)
   {
      puzzleIcons[puzzleNr].color = solvedGreen;
      RpcHandlePuzzleSolved(puzzleNr);
   }

   [ClientRpc]
   public void RpcHandlePuzzleSolved(int puzzleNr)
   {
      Debug.Log("HandlePuzzleSolved RPC was called");
      puzzleIcons[puzzleNr].color = solvedGreen;
   }

   private void OnEnable()
   {
      TrophyPuzzleController.PuzzleSolved += OnPuzzleSolved;
      ButtonPuzzleController.PuzzleSolved += OnPuzzleSolved;
      CandlePuzzleControler.PuzzleSolved += OnPuzzleSolved;
      PanelInput.PuzzleSolved += OnPuzzleSolved;
   }

   private void OnDisable()
   {
      TrophyPuzzleController.PuzzleSolved -= OnPuzzleSolved;
      ButtonPuzzleController.PuzzleSolved -= OnPuzzleSolved;
      CandlePuzzleControler.PuzzleSolved -= OnPuzzleSolved;
      PanelInput.PuzzleSolved -= OnPuzzleSolved;
   }
}
