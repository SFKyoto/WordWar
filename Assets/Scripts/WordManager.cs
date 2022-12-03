using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public AnswerManager guiAnswerManager;
    //public GUIIndicatorManager guiIndicatorManager;
    public GameGuessesManager wordsManager;
    public GUIKeyboardManager guiKeyboardManager;
    public GUIControl guiOptionsControl;

    [Header("Text Labels")]
    public TextMeshProUGUI TXTScore;
    public TextMeshProUGUI TXTTriesCount;
    public TextMeshProUGUI TXTSuccesfullGuesses;

    [Header("Round State")]
    public string currentGuess = "";
    public List<string> previousWords = new List<string>();
    private int currentScore = 0;
    private int currentTries = 0;
    private int successfulGuesses = 0;
    public bool isKeyboardLocked = false;

    [Header("Colors")]
    public Color defaultColor;
    public Color correct;
    public Color miss;
    public Color incorrect;
    public Color fontOnBlack;
    public Color fontOnWhite;
    
    private Color[] returnedColors;
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

        letterColorsDict.Add((char)AttempededLetter.Missed, miss);
        letterColorsDict.Add((char)AttempededLetter.NotInWord, incorrect);

        returnedColors = new Color[5];
    }
    void Update()
    {
        if (!isKeyboardLocked)
        {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
                guiOptionsControl.MenuToggle();
                return;
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
    }

    public void TypeLetter(char letter)
    {
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
            //guiIndicatorManager.MoveIndicator(currentGuess.Length*1.25f);
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
    /// Envia tentativa do usuário para wordsManager; para o singleplayer, o próximo passo é invocar ReceiveAttemptedLetters().
    /// </summary>
    private void ReviseLetters()
    {
        isKeyboardLocked = true;
        Debug.Log($"ReviseLetters - {currentGuess}");
        string checkedWord = wordsManager.GetCheckedAttempt(currentGuess);
        if(checkedWord != "") ReceiveAttemptedLetters(checkedWord);
    }

    /// <summary>
    /// Verifica se o resultado recebido é uma palavra acertada e gera a lógica de pintar teclas/letras.
    /// </summary>
    public void ReceiveAttemptedLetters(string checkedWord)
    {
        Debug.Log($"Recebendo {checkedWord}");
        Debug.Log($"currentGuess: {currentGuess}");
        bool allCorrect = true;
        if (checkedWord == "X")
        {
            ShakeLetters();
            allCorrect = false;
        }
        else
        {
            System.Text.StringBuilder checkedWordEditable = new System.Text.StringBuilder(checkedWord);
            for (int i = 0; i < checkedWord.Length; i++)
            {
                try
                {
                    returnedColors[i] = letterColorsDict[checkedWord[i]];
                    checkedWordEditable[i] = currentGuess[i];
                    allCorrect = false;
                }
                catch
                {
                    returnedColors[i] = correct;
                }
            }

            currentTries++;
            if (allCorrect)
            {
                previousWords.Add(checkedWord);
                IncreaseScore();
            }

            string currentGuessNoAccents = SinglePlayerTextManipulation.RemoveAccents(currentGuess);
            guiAnswerManager.CreateAnswerSprites(checkedWordEditable.ToString(), returnedColors, fontOnWhite);
            guiKeyboardManager.PaintKeyboardLetters(currentGuessNoAccents.ToUpper(), returnedColors);

            UpdateStatsText();
            ClearGuess(isAllCorrect: allCorrect);
            currentGuess = "";
        }
        isKeyboardLocked = false;
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
        currentScore += Math.Max(0, 1100 - (currentTries * 100));
        currentTries = 0;
    }

    private void UpdateStatsText()
    {
        TXTScore.text = "Pontos: " + currentScore.ToString();
        TXTTriesCount.text = "Tentativas: " + currentTries.ToString();
        TXTSuccesfullGuesses.text = "Acertos: " + successfulGuesses.ToString();
    }

    public void BecomeObserver()
    {
        Debug.Log("Virou observador");
        isKeyboardLocked = true;
        guiKeyboardManager.HideKeyboard();
        FindObjectOfType<GUIMultiplayerManager>().SLDTempoRestante.enabled = false;
        FindObjectOfType<GUIMultiplayerManager>().TXTObserverWarn.enabled = true;
    }
}
