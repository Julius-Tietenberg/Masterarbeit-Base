using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject introCard;
    [SerializeField] private GameObject connectCard;

    private bool candleTutorialDone;
    private bool codeTutorialDone;
    private bool swapTutorialDone;

    private void TutorialSolved(int tutorialNr)
    {
        if (tutorialNr == 1)
        {
            codeTutorialDone = true;
        }
        else if (tutorialNr == 2)
        {
            swapTutorialDone = true;
        }
        else if (tutorialNr == 3)
        {
            candleTutorialDone = true;
        }

        if (candleTutorialDone && codeTutorialDone && swapTutorialDone)
        {
            introCard.SetActive(false);
            connectCard.SetActive(true);
        }
    }


    private void OnEnable()
    {
        TutorialCandleController.CandleTutorialUsed += TutorialSolved;
        TutorialCodeController.CodeTutorialUsed += TutorialSolved;
        TutorialSwapController.SwapTutorialUsed += TutorialSolved;
    }

    private void OnDisable()
    {
        TutorialCandleController.CandleTutorialUsed -= TutorialSolved;
        TutorialCodeController.CodeTutorialUsed -= TutorialSolved;
        TutorialSwapController.SwapTutorialUsed -= TutorialSolved;
    }
}