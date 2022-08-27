using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordManager : MonoBehaviour
{
    public AnswerManager guiAnswerManager;
    public GUIIndicatorManager guiIndicatorManager;
    public SinglePlayerWordsManager wordsManager;
    public GUIKeyboardManager guiKeyboardManager;

    [Header("Text Objects")]
    public Text TXTScore;
    public Text TXTTriesCount;
    public Text TXTSuccesfullGuesses;

    [Header("Round State")]
    public string currentGuess = "";
    public string answerOfTurn = "cobra";
    public string answerOfTurnNoAccents;
    public List<string> previousWords = new List<string>();
    private int currentScore = 0;
    private int triesCount = 0;
    private int currentTries = 0;
    private int successfulGuesses = 0;

    [Header("Colors")]
    public Color defaultColor;
    public Color correct;
    public Color miss;
    public Color incorrect;
    public Color fontOnBlack;
    public Color fontOnWhite;
    
    private Color[] exits;

    private readonly string validLetters = "qwertyuiopasdfghjklçzxcvbnm";

    private void Start() 
    {
        //foreach(Transform child in transform)
        //{
        //    if(child != null)
        //    {
        //        GUIGuessedWordManager guiGessedWordManager = child.GetComponent<GUIGuessedWordManager>();
        //        guiGessedWordManager.SetFontColor(fontOnBlack);
        //    }
        //}
        //StartCoroutine(WaitForSeconds(0.5f));
        GenerateNewAnswer();
    }

    //IEnumerator WaitForSeconds(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    GenerateNewAnswer();
    //}

    void Update()
    {
        foreach (char letter in Input.inputString.ToLower())
        {
            if (letter == '\b')
            {
                DeleteLetter();
            }
            else if (letter == '\n' || letter == '\r')
            {
                EnterWord();
            }
            else if (validLetters.Contains(letter.ToString()))
            {
                TypeLetter(letter);
            }
        }
    }

    private void GenerateNewAnswer() 
    {
        answerOfTurn = wordsManager.GetRandomAnswer();
        previousWords.Add(answerOfTurn);
        answerOfTurnNoAccents = SinglePlayerTextManipulation.RemoveAccents(answerOfTurn);
        exits = new Color[answerOfTurnNoAccents.Length];
    }

    public void TypeLetter(char letter)
    {
        Debug.Log(currentGuess.Length);
        if(currentGuess.Length < 5)
        {
            currentGuess += letter;
            Transform letterTransf = transform.GetChild(currentGuess.Length - 1);
            GUIGuessedWordManager guiLetterManager = letterTransf.GetComponent<GUIGuessedWordManager>();
            //if(guiLetterManager.GetColor() == correctColor)
            guiLetterManager.SetLetter(letter);
            //guiIndicatorManager.MoveIndicator(currentGuess.Length*1.25f);
        }
    }

    public void DeleteLetter()
    {
        if(currentGuess.Length > 0)
        {
            currentGuess = currentGuess.Remove(currentGuess.Length-1);
            Transform letterTransf = transform.GetChild(currentGuess.Length);
            GUIGuessedWordManager letterControl = letterTransf.GetComponent<GUIGuessedWordManager>();
            letterControl.SetLetter(' ');
            guiIndicatorManager.MoveIndicator(currentGuess.Length*1.25f);
        }
    }

    public void EnterWord()
    {
        if(currentGuess.Length==5 && wordsManager.IsInList(currentGuess))
        {
            ReviseLetters();
        }
        else
        {
            //para GUI depois
            ShakeLetters();
        }
    }

    private void ShakeLetters()
    {
        foreach (Transform child in transform)
        {
            if (child != null)
            {
                iTween.ShakePosition(child.gameObject, new Vector3(1, 1, 1), 1.0f);
            }
        }
    }

    /*
    private string GetWord()
    {
        string currentGuessNoAccents = "";
        foreach(Transform child in transform)
        {
            LetterControl guiLetterManager = child.GetComponent<LetterControl>();
            if (guiLetterManager)
            {
                currentGuessNoAccents += guiLetterManager.GetLetter();
            }
        }
        currentGuessNoAccents = currentGuessNoAccents.ToLower();
        return currentGuessNoAccents;
    }
    */

    private void ReviseLetters()
    {
        string currentGuessNoAccents = SinglePlayerTextManipulation.RemoveAccents(currentGuess);
        string answerCopy = answerOfTurnNoAccents;
        int indexWordCopy = 0;

        //primeiro checamos quais letras s�o corretas
        Debug.Log(currentGuessNoAccents);
        Debug.Log(answerCopy);
        for(int i = 0; i < currentGuessNoAccents.Length; i++){
            if(currentGuessNoAccents[i] == answerOfTurnNoAccents[i]){
                exits[i] = correct;
                answerCopy = answerCopy.Remove(indexWordCopy, 1);
                Debug.Log("Correto na pos " + i.ToString());
                indexWordCopy--;
                Debug.Log("wordCopy agora: " + answerCopy);
            }
            indexWordCopy++;
        }

        //answerCopy s� tem as letras incorretas agora
        //ex: MYLLA + tentativa LLLAL = MYLA
        //vamos checar quais ganham cor 'amarela' ou 'cinza'
        Debug.Log("Palavra que sobrou: " + answerCopy);
        for(int i = 0; i < currentGuessNoAccents.Length; i++){
            if(currentGuessNoAccents[i] != answerOfTurnNoAccents[i]){
                int indexLetraQuase = answerCopy.IndexOf(currentGuessNoAccents[i]);
                if(indexLetraQuase != -1){
                    exits[i] = miss;
                    Debug.Log("Quase na pos " + i.ToString());
                    answerCopy = answerCopy.Remove(indexLetraQuase, 1);
                }
                else{
                    exits[i] = incorrect;
                    Debug.Log("Erradissimo na pos " + i.ToString());
                }
            }
        }

        bool allCorrect = true;
        Debug.Log(currentGuessNoAccents);
        foreach(Color exit in exits)
        {
            if(exit != correct) 
            {
                allCorrect = false;
            }
        }

        guiAnswerManager.CreateAnswerSprites(allCorrect ? previousWords[previousWords.Count-1] : currentGuessNoAccents,exits,fontOnWhite);

        guiKeyboardManager.PaintKeyboardLetters(currentGuessNoAccents.ToUpper(),exits);

        //Debug.Log("All correctColor: ");
        //Debug.Log(allCorrect);
        if(allCorrect)
        {
            IncreaseScore();
            GenerateNewAnswer();
        }
        else
        {
            triesCount++;
            currentTries++;
        }
        ClearGuess(isAllCorrect: allCorrect);

        UpdateStatsText();
        currentGuess = "";
    }

    /*
    private void CheckPreviouslyGuessedLetters()
    {
        Transform child = transform.Find("Letter"+currentGuess.Length.ToString());
        if(child != null)
        {
            LetterControl guiLetterManager = child.GetComponent<LetterControl>();
            if(guiLetterManager.GetColor() == correctColor)
            {
                currentGuess += answerOfTurnNoAccents.ToCharArray()[letterIndex];
                guiLetterManager.SetLetter(answerOfTurnNoAccents.ToCharArray()[letterIndex]);
                CheckPreviouslyGuessedLetters();
            }
        }
    }
    */

    private void ClearGuess(bool isAllCorrect)
    {
        if(!isAllCorrect)
        {
            foreach(Transform child in transform)
            {
                GUIGuessedWordManager letterControl = child.GetComponent<GUIGuessedWordManager>();
                if (letterControl && !letterControl.isCorrect)
                {
                    letterControl.SetLetter(' ');
                }
            }
        }
        else
        {
            foreach(Transform child in transform)
            {
                GUIGuessedWordManager letterControl = child.GetComponent<GUIGuessedWordManager>();
                if (letterControl)
                {
                    letterControl.SetLetter(' ');
                    letterControl.isCorrect = false;
                    letterControl.SetBgColor(defaultColor);
                }
            }
            guiKeyboardManager.ResetColors();
        }
    }

    private void IncreaseScore()
    {
        successfulGuesses++;
        currentScore += currentTries <= 0 ? 1000 : 1000/currentTries;
        currentTries = 0;
    }

    private void UpdateStatsText()
    {
        TXTScore.text = "Pontos: " + currentScore.ToString();
        TXTTriesCount.text = "Tentativas: " + triesCount.ToString();
        TXTSuccesfullGuesses.text = "Acertos: " + successfulGuesses.ToString();
    }
}
