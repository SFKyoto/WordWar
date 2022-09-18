using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordManager : MonoBehaviour
{
    public AnswerManager guiAnswerManager;
    public GUIIndicatorManager guiIndicatorManager;
    public GameGuessesManager wordsManager;
    public GUIKeyboardManager guiKeyboardManager;
    public GUIControl guiControl;

    [Header("Text Objects")]
    public Text TXTScore;
    public Text TXTTriesCount;
    public Text TXTSuccesfullGuesses;

    [Header("Round State")]
    public string currentGuess = "";
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
    private Dictionary<char, Color> letterColorsDict = new Dictionary<char, Color>();

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

        letterColorsDict.Add('C', correct);
        letterColorsDict.Add('M', miss);
        letterColorsDict.Add('I', incorrect);

        exits = new Color[5];
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            guiControl.MenuToggle();
        }
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

    /// <summary>
    /// Remove a última letra digitada da memória e da interface.
    /// </summary>
    private void DeleteLetter()
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

    private void EnterWord()
    {
        if(currentGuess.Length==5)
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

    /// <summary>
    /// Envia tentativa do usuário para wordsManager e verifica se o resultado recebido é uma palavra acertada.
    /// </summary>
    private void ReviseLetters()
    {
        bool allCorrect = true;
        Debug.Log(currentGuess);
        string checkedWord = wordsManager.GetCheckedAttempt(currentGuess);
        Debug.Log("attempt string chain: " + new string(checkedWord));
        if (checkedWord == "X")
        {
            ShakeLetters();
            allCorrect = false;
        }
        else
        {
            for (int i = 0; i < checkedWord.Length; i++)
            {
                exits[i] = letterColorsDict[checkedWord[i]];
                allCorrect = allCorrect && (checkedWord[i] == 'C');
            }

            if (allCorrect)
            {
                previousWords.Add(currentGuess);
                IncreaseScore();
            }
            else
            {
                triesCount++;
                currentTries++;
            }

            string currentGuessNoAccents = SinglePlayerTextManipulation.RemoveAccents(currentGuess);
            guiAnswerManager.CreateAnswerSprites(allCorrect ? previousWords[previousWords.Count - 1] : currentGuessNoAccents, exits, fontOnWhite);
            guiKeyboardManager.PaintKeyboardLetters(currentGuessNoAccents.ToUpper(), exits);

            UpdateStatsText();
            ClearGuess(isAllCorrect: allCorrect);
            currentGuess = "";
        }
    }

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
