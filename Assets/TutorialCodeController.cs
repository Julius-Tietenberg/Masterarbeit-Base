using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialCodeController : MonoBehaviour
{
    
    public static event Action<int> CodeTutorialUsed;

    public AudioSource audioSource;
    public AudioClip error;
    public AudioClip buttonPress;
    public float volume;

    [SerializeField] private Animator lockAnimator;

    [SerializeField]
    private bool codePuzzleSolved;
    
    [SerializeField] 
    private TMP_Text displayTextObject;
    
    [SerializeField] 
    private Image statusLed;

    [SerializeField]
    private string currentLetter;

    public bool puzzleActive; 
    
    
    public void EnterLetter(string letter)
    {
        Debug.Log("Switch the last entered Letter");
        currentLetter += letter;
        displayTextObject.text = currentLetter;
        audioSource.PlayOneShot(buttonPress, volume);
        CodeTutorialUsed?.Invoke(1);
    }
    
    private void Update()
    {
        if (currentLetter.Length == 3)
        {
            currentLetter = "";
        }
    }
}
