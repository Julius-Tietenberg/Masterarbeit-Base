using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ButtonPuzzleController : NetworkBehaviour
{

    public List<GameObject> buttonsInSequence;


    [SyncVar]
    public int currentButtonValue;
    
    public int maxButtonValue;

    [SyncVar] 
    public bool timeExceeded = true;
    
    [SyncVar] 
    public bool buttonPuzzleSolved;

    public void CheckSequence(int newValue)
    {
        if (buttonPuzzleSolved)
        {
            return;
        }
        if (newValue == 1 && currentButtonValue == 0)
        {
            currentButtonValue = newValue;
            buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonCorrect();
            timeExceeded = false;
            StartCoroutine(WaitAndResetBool(5));
            //Give some feedback that the button was correct and the next one needs to be pressed within 5secs
        }
        else if (newValue == (currentButtonValue + 1) && newValue == maxButtonValue && !timeExceeded)
        {
            currentButtonValue = newValue;
            buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonCorrect();
            buttonPuzzleSolved = true;
            // Give feedback that the puzzle is solved and deactivate new inputs
        }
        else if (newValue == (currentButtonValue + 1) && !timeExceeded)
        {
            currentButtonValue = newValue;
            buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonCorrect();
            timeExceeded = false;
            StartCoroutine(WaitAndResetBool(5));
            //Give some feedback that the button was correct and the next one needs to be pressed within 5secs
        }
        else
        {
            buttonsInSequence[newValue-1].GetComponent<ButtonController>().FeedbackButtonFalse();
            
            foreach (var button in buttonsInSequence)
            {
                button.GetComponent<ButtonController>().ResetButtonState();
                currentButtonValue = 0;
                timeExceeded = true;
                //Give some feedback that the puzzle was reset
            }
        }
    }

    private IEnumerator WaitAndResetBool(int secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        timeExceeded = true;
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