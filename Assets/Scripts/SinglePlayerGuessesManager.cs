using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SinglePlayerGuessesManager : GameGuessesManager
{
    private void Start()
    {
        GenerateNewAnswer();
    }

    public override void GenerateNewAnswer()
    {
        if (listPossibleAnswers.Count == 0) GetWordLists();
        answerOfTurn = GetRandomAnswerFromList();
        answerOfTurnNoAccents = SinglePlayerTextManipulation.RemoveAccents(answerOfTurn);
    }
    public override string GetCheckedAttempt(string currentGuess)
    {
        string checkedAttempt = CompareWordWithGuess(currentGuess, answerOfTurn);
        if (checkedAttempt != "X" && !checkedAttempt.Contains((char)AttempededLetter.Missed) && !checkedAttempt.Contains((char)AttempededLetter.NotInWord))
        {
            GenerateNewAnswer();
        }
        return checkedAttempt;
    }

}
