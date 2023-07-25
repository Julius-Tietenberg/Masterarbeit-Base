using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Mirror;

public class DataLogging : NetworkBehaviour
{

    private string SAVE_FOLDER;
    
    private string filename = "/PlayerData.csv";

    [SerializeField] private GameFlowManager gameFlowManager;

    public DateTime datetime;
    
    public Transform posPlayer1;
    public Transform posPlayer2;  // CLEAR when testing with two headsets
    // public List<GameObject> playerHeads;
    public float distanceBetweenPlayers;
    public float sumOfDistances;
    
    public int totalTimeSecs;
    
    public int timeInIntimateSpace; // Distance of 0.45m or less
    public int timeInPersonalSpace; // Distance of 1.2m or less
    public int timeInSocialSpace; // Distance of 3.7m or less
    public int timeInPublicSpace; // Distance above 3.7m
    
    public int percentageInIntimateSpace; // Distance of 0.45m or less
    public int percentageInPersonalSpace; // Distance of 1.2m or less
    public int percentageInSocialSpace; // Distance of 3.7m or less
    public int percentageInPublicSpace; // Distance above 3.7m
    
    public float averageDistance;
    public float minimumDistance;
    
    public int timestampCodePuzzleSolved;
    public int timestampCandlePuzzleSolved;
    public int timestampButtonPuzzleSolved;
    public int timestampTrophyPuzzleSolved;


    public void StartDataLogging()
    {
        if (isServer)
        {
           datetime = DateTime.Now;
           CheckDirectory();
           GameObject[] avatarHeads =  GameObject.FindGameObjectsWithTag("Avatar Head");
           if (avatarHeads.Length == 1)
           {
               posPlayer1 = avatarHeads[0].transform;
           }
           else if (avatarHeads.Length == 2)
           {
               posPlayer1 = avatarHeads[0].transform;
               posPlayer2 = avatarHeads[1].transform;
           }
           InvokeRepeating("UpdateStudyData", 1f, 1f); 
        }
    }

    // Should be run once a second during the gameplay session.
    public void UpdateStudyData()
    {
        Debug.Log("UpdateStudy data was called (and should be, every second)");
        totalTimeSecs += 1;

        if (posPlayer1 && posPlayer2)
        {
            distanceBetweenPlayers = Vector3.Distance(posPlayer1.position, posPlayer2.position);

            sumOfDistances += distanceBetweenPlayers;
            averageDistance = (sumOfDistances / totalTimeSecs);
            
            if (distanceBetweenPlayers < minimumDistance)
            {
                // Initial value???
                minimumDistance = distanceBetweenPlayers;
            }
            else if (minimumDistance == 0)
            {
                minimumDistance = distanceBetweenPlayers;
            }
            
            if (distanceBetweenPlayers <= 0.45f)
            {
                timeInIntimateSpace += 1;
            }
            else if (distanceBetweenPlayers <= 1.2f)
            {
                timeInPersonalSpace += 1;
            }
            else if (distanceBetweenPlayers <= 3.7)
            {
                timeInSocialSpace += 1;
            }
            else if (distanceBetweenPlayers > 3.7)
            {
                timeInPublicSpace += 1;
            }
            else
            {
                Debug.Log("It was not possible to categorize the distance for some reason.");
            }
            CalculatePercentages();
        }
    }

    // calculate at the end
    public void CalculatePercentages()
    {
        if (timeInIntimateSpace == 0)
        {
            percentageInIntimateSpace = 0;
        }
        else
        {
            percentageInIntimateSpace = ((timeInIntimateSpace / totalTimeSecs) * 100);
        }
        
        if (timeInPersonalSpace == 0)
        {
            percentageInPersonalSpace = 0;
        }
        else
        {
            percentageInPersonalSpace = ((timeInPersonalSpace / totalTimeSecs) * 100);
        }
        
        if (timeInSocialSpace == 0)
        {
            percentageInSocialSpace = 0;
        }
        else
        {
            percentageInSocialSpace = ((timeInSocialSpace / totalTimeSecs) * 100);
        }
        
        if (timeInPublicSpace == 0)
        {
            percentageInPublicSpace = 0;
        }
        else
        {
            percentageInPublicSpace = ((timeInPublicSpace / totalTimeSecs) * 100);
        }
    }

    public void SetPuzzleTimestamp(PuzzleType type)
    {
        if (type == PuzzleType.Button)
        {
            timestampButtonPuzzleSolved = totalTimeSecs;
        }
        else if (type == PuzzleType.Candle)
        {
            timestampCandlePuzzleSolved = totalTimeSecs;
        }
        else if (type == PuzzleType.Trophy)
        {
            timestampTrophyPuzzleSolved = totalTimeSecs;
        }
        else if (type == PuzzleType.Code)
        {
            timestampCodePuzzleSolved = totalTimeSecs;
        }
    }
    
    
    void SetFilePath()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            SAVE_FOLDER = Application.persistentDataPath + "/Results";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer ||
                 Application.platform == RuntimePlatform.WindowsEditor)
        {
            SAVE_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Results";
        }
    }
    
    public void CheckDirectory() //Check if the filepath exists and if not, create it
    {
        SetFilePath();
        if (SAVE_FOLDER != "no")
        {
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }

        CheckIfFileExists();
    }

    public void CheckIfFileExists()
    {
        if (!File.Exists(SAVE_FOLDER + filename))
        {
            CreateSaveFile();
        }
    }

    public void CreateSaveFile()
    {
        TextWriter tw = new StreamWriter(SAVE_FOLDER + filename, true);
        tw.WriteLine(
            "DateTime; Total Session Time (secs); Average Distance (m); Minimum Distance (m); Time in Intimate Space (sec); Time in Personal Space (sec); Time in Social Space (sec); Time in Public Space (sec); Percentage in Intimate Space (sec); Percentage in Personal Space (sec); Percentage in Social Space (sec); Percentage in Public Space (sec); Timestamp Candle Puzzle; Timestamp Code Puzzle; Timestamp Trophy Puzzle; Timestamp Button Puzzle;");
        tw.Close();
    }

    public void SaveData()
    {
        if (isServer)
        {
            CalculatePercentages();
                    
            TextWriter tw = new StreamWriter(SAVE_FOLDER + filename, true);
            tw.WriteLine(datetime + " ; " + totalTimeSecs + " ; " + averageDistance + " ; " +
                                 minimumDistance + " ; " + timeInIntimateSpace + " ; " +
                                 timeInPersonalSpace + " ; " + timeInSocialSpace + " ; " + timeInPublicSpace + " ; " +
                                 percentageInIntimateSpace + " ; " + percentageInPersonalSpace + " ; " + percentageInSocialSpace +
                                 " ; " + percentageInPublicSpace +
                                 " ; " + timestampCandlePuzzleSolved + " ; " + timestampCodePuzzleSolved + " ; " +
                                 timestampTrophyPuzzleSolved +
                                 " ; " + timestampButtonPuzzleSolved + " ; ");
            tw.Close();
        }
    }


    private void OnEnable()
    {
        GameFlowManager.NewPuzzleSolved += SetPuzzleTimestamp;
        GameFlowManager.StartDataLogging += StartDataLogging;
        GameFlowManager.StopDataLogging += SaveData;
    }

    private void OnDisable()
    {
        GameFlowManager.NewPuzzleSolved -= SetPuzzleTimestamp;
        GameFlowManager.StartDataLogging -= StartDataLogging;
        GameFlowManager.StopDataLogging += SaveData;
    }

}
