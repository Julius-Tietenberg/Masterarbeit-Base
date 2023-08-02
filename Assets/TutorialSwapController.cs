using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSwapController : MonoBehaviour
{
    public static event Action<int> SwapTutorialUsed;

    public AudioSource audioSource;
    public AudioClip swoosh;
    public float volume;
    
    [SerializeField] public Vector3 positionOne;
    [SerializeField] public Vector3 positionTwo;

    [SerializeField] public List<Vector3> trophyPositions; 

    [SerializeField] private List<GameObject> currentOrder;

    public bool swapUsed;
    
    private void Awake()
    {
        trophyPositions[0] = currentOrder[0].transform.position;
        trophyPositions[1] = currentOrder[1].transform.position;
    }
    
    public void SwitchTrophyPositions()
    {
        if (!swapUsed)
        {
            swapUsed = true;
            SwapTutorialUsed?.Invoke(2);
        }
        audioSource.PlayOneShot(swoosh, volume);
            
        var leftTrophy = currentOrder[0]; 
        var rightTrophy = currentOrder[1];
        currentOrder[0] = rightTrophy;
        currentOrder[1] = leftTrophy;
        
        var counter = 0;
        foreach (var trophy in currentOrder)
        {
            trophy.transform.position = trophyPositions[counter];
            counter++;
        }
    }
}
