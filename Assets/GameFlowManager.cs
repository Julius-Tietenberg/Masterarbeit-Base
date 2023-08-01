using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

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
   public static event Action EndCandlePuzzle;
   public static event Action StartDataLogging;
   public static event Action StartGame;
   public static event Action StopDataLogging;

   [SerializeField] private Animator doorAnimatorRight;
   [SerializeField] private Animator doorAnimatorLeft;
   [SerializeField] private Animator lockAnimator;

   [SerializeField] private List<Image> puzzleIcons;

   [SerializeField] private Color solvedGreen;

   [SerializeField] private GameObject startGameScreen;
   [SerializeField] private GameObject endGameScreen;
   [SerializeField] private TMP_Text endGameText;
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
   public AudioSource soundtrackSource;
   public AudioSource clockSource;
   public AudioClip clip;
   public AudioClip soundtrack;
   public AudioClip gameOver;
   public float volume=0.6f;


   public void Awake()
   {
      soundtrackSource.Stop();
      endGameScreen.SetActive(false);
   }


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
         endGameScreen.SetActive(false);
         StartDataLogging?.Invoke();
         StartGame?.Invoke();
         audioSource.PlayOneShot(gameOver, 0.8f);
         soundtrackSource.Play();
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
      audioSource.PlayOneShot(gameOver, 0.8f);
      startGameScreen.SetActive(false);
      endGameScreen.SetActive(false);
      soundtrackSource.Play();
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
         EndCandlePuzzle?.Invoke();
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
      soundtrackSource.Stop();
      clockSource.volume = 0f;
      clockSource.Stop();
      audioSource.PlayOneShot(gameOver, 0.8f);
      if (amountOfSolvedPuzzles == 4 && gameFinishedByPuzzles)
      {
         // Logic for Win - Solved by Puzzles
         endGameText.text = "Es wurden " + amountOfSolvedPuzzles + "/4 Rätseln erfolgreich gelöst. <br>Bitte wenden Sie sich jetzt an den/die Versuchsleiter*in.";
         endGameScreen.SetActive(true);
         doorAnimatorLeft.SetTrigger("isOpenDoor");
         doorAnimatorRight.SetTrigger("isOpenDoor");
         lockAnimator.SetTrigger("isLockOpen");
         RpcEndGame(amountOfSolvedPuzzles);
         
      }
      else
      {
         // Logic for End - By time, X Puzzles solved
         endGameText.text = "Es wurden " + amountOfSolvedPuzzles + "/4 Rätseln erfolgreich gelöst. <br>Bitte wenden Sie sich jetzt an den/die Versuchsleiter*in.";
         endGameScreen.SetActive(true);
         doorAnimatorLeft.SetTrigger("isOpenDoor");
         doorAnimatorRight.SetTrigger("isOpenDoor");
         lockAnimator.SetTrigger("isLockOpen");
         RpcEndGame(amountOfSolvedPuzzles);
         
      }
   }

   [ClientRpc]
   public void RpcEndGame(int solvedPuzzles)
   {
      soundtrackSource.Stop();
      clockSource.volume = 0f;
      clockSource.Stop();
      audioSource.PlayOneShot(gameOver, 0.8f);
      endGameText.text = "Es wurden " + solvedPuzzles + "/4 Rätseln erfolgreich gelöst. <br>Bitte wenden Sie sich jetzt an den/die Versuchsleiter*in.";
      endGameScreen.SetActive(true);
      doorAnimatorLeft.SetTrigger("isOpenDoor");
      doorAnimatorRight.SetTrigger("isOpenDoor");
      lockAnimator.SetTrigger("isLockOpen");
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
