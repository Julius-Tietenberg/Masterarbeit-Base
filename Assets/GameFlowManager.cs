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
   
   public static event Action<PuzzleType> NewPuzzleSolved;
   public static event Action StartDataLogging;
   public static event Action StopDataLogging;

   [SerializeField] private Animator doorAnimatorRight;
   [SerializeField] private Animator doorAnimatorLeft;
   [SerializeField] private Animator lockAnimator;

   [SerializeField] private List<Image> puzzleIcons;

   [SerializeField] private Color solvedGreen;

   [SerializeField] private GameObject startGameScreen;
   [SerializeField] private Image buttonPlayer1;
   [SerializeField] private Image buttonPlayer2;

   public bool trophyPuzzleSolved;
   public bool candlePuzzleSolved;
   public bool codePuzzleSolved;
   public bool buttonPuzzleSolved;

   public int amountOfSolvedPuzzles;
   public bool gameFinishedByPuzzles;

   [SyncVar] public bool readyPlayer1;
   [SyncVar] public bool readyPlayer2;
   
   public AudioSource audioSource;
   public AudioClip clip;
   public float volume=0.6f;
   
   
   [Command (requiresAuthority = false)]
   public void CmdSetPlayerStatus(int player)
   {
      if (player == 1)
      {
         readyPlayer1 = !readyPlayer1;
         
         if (readyPlayer1)
         {
            buttonPlayer1.color = solvedGreen;
         }
      }
      else if (player == 2)
      {
         readyPlayer2 = !readyPlayer2;
         
         if (readyPlayer2)
         {
            buttonPlayer2.color = solvedGreen;
         }
      }
      RpcSetButton();

      if (readyPlayer1 && readyPlayer2)
      {
         startGameScreen.SetActive(false);
         StartDataLogging?.Invoke();
         RpcStartGame();
         // Start the countdown and timer
         // Start data logging
         
      }
   }

   [ClientRpc]
   public void RpcSetButton()
   {
      if (readyPlayer1)
      {
         buttonPlayer1.color = solvedGreen;
      }
      
      if (readyPlayer2)
      {
         buttonPlayer2.color = solvedGreen;
      }
   }
   
   [ClientRpc]
   public void RpcStartGame()
   {
      startGameScreen.SetActive(false);
   }

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
      NewPuzzleSolved?.Invoke(type);
   }
   
   public void HandlePuzzleFinished(int puzzleNr)
   {
      puzzleIcons[puzzleNr].color = solvedGreen;
      audioSource.PlayOneShot(clip, volume);
      amountOfSolvedPuzzles += 1;
      RpcHandlePuzzleSolved(puzzleNr);
      CheckIfAllPuzzlesSolved();
   }

   public void CheckIfAllPuzzlesSolved()
   {
      if (amountOfSolvedPuzzles == 4 && !gameFinishedByPuzzles)
      {
         gameFinishedByPuzzles = true;
         StopDataLogging?.Invoke();
         EndGame();
      }
   }

   [ClientRpc]
   public void RpcHandlePuzzleSolved(int puzzleNr)
   {
      Debug.Log("HandlePuzzleSolved RPC was called");
      audioSource.PlayOneShot(clip, volume);
      puzzleIcons[puzzleNr].color = solvedGreen;
   }

   public void EndGame()
   {
      if (amountOfSolvedPuzzles == 4 && gameFinishedByPuzzles)
      {
         // Logic for Win - Solved by Puzzles
         
         Debug.Log("DoorOpen Bool Left before =" + doorAnimatorLeft.GetBool("DoorOpen"));
         Debug.Log("DoorOpen Bool Right before =" + doorAnimatorRight.GetBool("DoorOpen"));
         //doorAnimatorLeft.SetBool("DoorOpen", true);
         //doorAnimatorRight.SetBool("DoorOpen", true);
         doorAnimatorLeft.SetTrigger("isOpenDoor");
         doorAnimatorRight.SetTrigger("isOpenDoor");
         lockAnimator.SetTrigger("isLockOpen");
         RpcEndGame();
         Debug.Log("Set bools for Door Anmiation");
         Debug.Log("DoorOpen Bool Left afterwards =" + doorAnimatorLeft.GetBool("DoorOpen"));
         Debug.Log("DoorOpen Bool Right afterwards =" + doorAnimatorRight.GetBool("DoorOpen"));
      }
      else
      {
         // Logic for End - By time, X Puzzles solved
         
         Debug.Log("DoorOpen Bool Left before =" + doorAnimatorLeft.GetBool("DoorOpen"));
         Debug.Log("DoorOpen Bool Right before =" + doorAnimatorRight.GetBool("DoorOpen"));
         //doorAnimatorLeft.SetBool("DoorOpen", true);
         //doorAnimatorRight.SetBool("DoorOpen", true);
         doorAnimatorLeft.SetTrigger("isOpenDoor");
         doorAnimatorRight.SetTrigger("isOpenDoor");
         lockAnimator.SetTrigger("isLockOpen");
         RpcEndGame();
         Debug.Log("Set bools for Door Anmiation");
         Debug.Log("DoorOpen Bool Left afterwards =" + doorAnimatorLeft.GetBool("DoorOpen"));
         Debug.Log("DoorOpen Bool Right afterwards =" + doorAnimatorRight.GetBool("DoorOpen"));
      }
   }

   [ClientRpc]
   public void RpcEndGame()
   {
      //doorAnimatorLeft.SetBool("DoorOpen", true);
      //doorAnimatorRight.SetBool("DoorOpen", true);
      
      doorAnimatorLeft.SetTrigger("isOpenDoor");
      doorAnimatorRight.SetTrigger("isOpenDoor");
      lockAnimator.SetTrigger("isLockOpen");
      Debug.Log("Set bools for Door Anmiation on Client");
   }

   private void OnEnable()
   {
      TrophyPuzzleController.PuzzleSolved += OnPuzzleSolved;
      ButtonPuzzleController.PuzzleSolved += OnPuzzleSolved;
      CandlePuzzleControler.PuzzleSolved += OnPuzzleSolved;
      PanelInput.PuzzleSolved += OnPuzzleSolved;
      DataLogging.ReachedEndOfPlaytime += EndGame;
   }

   private void OnDisable()
   {
      TrophyPuzzleController.PuzzleSolved -= OnPuzzleSolved;
      ButtonPuzzleController.PuzzleSolved -= OnPuzzleSolved;
      CandlePuzzleControler.PuzzleSolved -= OnPuzzleSolved;
      PanelInput.PuzzleSolved -= OnPuzzleSolved;
      DataLogging.ReachedEndOfPlaytime -= EndGame;
   }
}
