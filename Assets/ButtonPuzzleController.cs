using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ButtonPuzzleController : NetworkBehaviour
{

    public static event Action<PuzzleType> PuzzleSolved;
    
    public AudioSource audioSource;
    public AudioClip clip;
    public AudioClip lastBeep;
    public float volume;

    [SerializeField] private Animator lockAnimator;
    
    // This list contains all Buttons (in order!).
    public List<GameObject> buttonsInSequence;

    // The current button that was pressed. Zero if none was pressed yet or when the puzzle resets.
    [SyncVar]
    public int currentButtonValue;
    
    // Total amount of buttons in the sequence.
    public int maxButtonValue;

    // Signals if the time to press the follow up button has passed. This is not used for the first button.
    [SyncVar] 
    public bool timeExceeded = true;
    
    // Will be set to true, once the puzzle is solved. 
    [SyncVar] 
    public bool buttonPuzzleSolved;

    // Will be set to true, while the puzzle resets after a wrong entry. 
    [SyncVar] public bool newInputBlocked;
    public void CheckSequence(int newValue)
    {
        if (buttonPuzzleSolved)
        {
            return;
        }
        if (newValue == 1 && currentButtonValue == 0 && !newInputBlocked)
        {
            currentButtonValue = newValue;
            buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonCorrect();
            timeExceeded = false;
            Debug.Log("Initial button pressed");
            StopAllCoroutines();
            StartCoroutine(WaitAndResetBool(5));
            //Give some feedback that the button was correct and the next one needs to be pressed within 5secs
        }
        else if (newValue == (currentButtonValue + 1) && newValue == maxButtonValue && !timeExceeded && !newInputBlocked)
        {
            currentButtonValue = newValue;
            buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonCorrect();
            buttonPuzzleSolved = true;
            PuzzleSolved?.Invoke(PuzzleType.Button);
            StopAllCoroutines();
            lockAnimator.SetTrigger("isLockOpen");
            RpcSolvedFeedback();
            Debug.Log("Button Sequence fully solved");
            // Give feedback that the puzzle is solved and deactivate new inputs
        }
        else if (newValue == (currentButtonValue + 1) && !timeExceeded && !newInputBlocked)
        {
            currentButtonValue = newValue;
            buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonCorrect();
            timeExceeded = false;
            Debug.Log("Button" + newValue + " was pressed in time");
            StopAllCoroutines();
            StartCoroutine(WaitAndResetBool(5));
            //Give some feedback that the button was correct and the next one needs to be pressed within 5secs
        }
        else
        {
            newInputBlocked = true;
            StopAllCoroutines();
            StartCoroutine(WrongFeedbackAndReset(newValue));
        }
    }

    [ClientRpc]
    public void RpcSolvedFeedback()
    {
        lockAnimator.SetTrigger("isLockOpen");
    }

    private IEnumerator WaitAndResetBool(int secondsToWait)
    {
        audioSource.PlayOneShot(audioSource.clip, volume);
        RpcPlayAudio(false);
        yield return new WaitForSeconds(secondsToWait / 4);
        audioSource.PlayOneShot(audioSource.clip, volume);
        RpcPlayAudio(false);
        yield return new WaitForSeconds(secondsToWait / 4);
        audioSource.PlayOneShot(audioSource.clip, volume);
        RpcPlayAudio(false);
        yield return new WaitForSeconds(secondsToWait / 4);
        audioSource.PlayOneShot(audioSource.clip, volume);
        RpcPlayAudio(false);
        yield return new WaitForSeconds(secondsToWait / 4);
        audioSource.PlayOneShot(lastBeep, volume);
        RpcPlayAudio(true);
        timeExceeded = true;
    }

    [ClientRpc]
    public void RpcPlayAudio(bool last)
    {
        if (!last)
        {
            audioSource.PlayOneShot(audioSource.clip, volume);
        }
        else
        {
            audioSource.PlayOneShot(lastBeep, volume);
        }
    }

    private IEnumerator WrongFeedbackAndReset(int newValue)
    {
        buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonFalse();
        Debug.Log("Wrong Button pressed.");

        yield return new WaitForSeconds(2);
        Debug.Log("Buttons will be reset now");
            
        foreach (var button in buttonsInSequence)
        {
            button.GetComponent<ButtonController>().ResetButtonState();
            //Give some feedback that the puzzle was reset
        }
        currentButtonValue = 0;
        newInputBlocked = false;
    }

    private void OnEnable()
    {
        ButtonController.ButtonWasPressed += CheckSequence;
    }

    private void OnDisable()
    {
        ButtonController.ButtonWasPressed -= CheckSequence;
    }
}