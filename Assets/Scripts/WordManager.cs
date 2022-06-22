using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordManager : MonoBehaviour
{
    public AnswerManager answerManager;
    public IndicatorManager indicatorManager;
    public WordRandomizer wordRandomizer;
    public KeyboardManager keyboardManager;

    [Header("Text Objects")]
    public Text score;
    public Text allTriesText;
    public Text allCorrects;

    [Header("Round State")]
    public string currentGuess = "";
    public string answerOfTurn = "cobra";
    public List<string> previousWords = new List<string>();
    private int scorePoints = 0;
    private int allTries = 0;
    private int tries = 0;
    private int hits = 0;

    [Header("Colors")]
    public Color defaultColor;
    public Color correct;
    public Color miss;
    public Color incorrect;
    public Color fontOnBlack;
    public Color fontOnWhite;
    
    private Color[] exits;

    private void Start() 
    {
        foreach(Transform child in transform)
        {
            if(child != null)
            {
                LetterControl script = child.GetComponent<LetterControl>();
                script.SetFontColor(fontOnBlack);
            }
        }
        StartCoroutine(WaitForSeconds(0.5f));
    }

    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        GenerateNewAnswer();
    }

    private void GenerateNewAnswer() 
    {
        string newWord = wordRandomizer.GetRandomAnswer();
        previousWords.Add(newWord);
        answerOfTurn = TextManipulation.RemoveAccents(newWord);
        exits = new Color[answerOfTurn.Length];
    }

    public void TypeLetter(char letter)
    {
        Debug.Log(currentGuess.Length);
        if(currentGuess.Length < 5)
        {
            currentGuess += letter;
            Transform letterTransf = transform.GetChild(currentGuess.Length - 1);
            LetterControl letterControl = letterTransf.GetComponent<LetterControl>();
            //if(letterControl.GetColor() == correct)
            letterControl.SetLetter(letter);
            indicatorManager.MoveIndicator(currentGuess.Length*1.25f);
            // CheckPreviouslyGuessedLetters();
        }
    }

    public void DeleteLetter()
    {
        if(currentGuess.Length > 0)
        {
            currentGuess = currentGuess.Remove(currentGuess.Length-1);
            Transform letterTransf = transform.GetChild(currentGuess.Length);
            LetterControl letterControl = letterTransf.GetComponent<LetterControl>();
            letterControl.SetLetter(' ');
            indicatorManager.MoveIndicator(currentGuess.Length*1.25f);
        }
    }

    public void EnterWord()
    {
        if(currentGuess.Length==5 && wordRandomizer.IsInList(currentGuess))
        {
            ReviseLetters();
        }
        else
        {
            foreach(Transform child in transform)
            {
                if(child != null)
                {
                    iTween.ShakePosition(child.gameObject,new Vector3(1,1,1),1.0f);
                }
            }
        }
    }

    /*
    private string GetWord()
    {
        string letters = "";
        foreach(Transform child in transform)
        {
            LetterControl letterControl = child.GetComponent<LetterControl>();
            if (letterControl)
            {
                letters += letterControl.GetLetter();
            }
        }
        letters = letters.ToLower();
        return letters;
    }
    */

    private void ReviseLetters()
    {
        string letters = TextManipulation.RemoveAccents(currentGuess);
        string wordCopy = answerOfTurn;
        int indexWordCopy = 0;

        //primeiro checamos quais letras s�o corretas
        for(int i = 0; i < letters.Length; i++){
            if(letters[i] == answerOfTurn[i]){
                exits[i] = correct;
                wordCopy = wordCopy.Remove(indexWordCopy, 1);
                Debug.Log("Correto na pos " + i.ToString());
                indexWordCopy--;
                Debug.Log("wordCopy aagora: " + wordCopy);
            }
            indexWordCopy++;
        }

        //wordCopy s� tem as letras incorretas agora
        //ex: MYLLA + tentativa LLLAL = MYLA
        //vamos checar quais ganham cor 'amarela' ou 'cinza'
        Debug.Log("Palavra que sobrou: " + wordCopy);
        for(int i = 0; i < letters.Length; i++){
            if(letters[i] != answerOfTurn[i]){
                int indexLetraQuase = wordCopy.IndexOf(letters[i]);
                if(indexLetraQuase != -1){
                    exits[i] = miss;
                    Debug.Log("Quase na pos " + i.ToString());
                    wordCopy = wordCopy.Remove(indexLetraQuase, 1);
                }
                else{
                    exits[i] = incorrect;
                    Debug.Log("Erradissimo na pos " + i.ToString());
                }
            }
        }

        bool allCorrect = true;
        Debug.Log(letters);
        foreach(Color exit in exits)
        {
            if(exit != correct) 
            {
                allCorrect = false;
            }
        }

        answerManager.CreateAnswer(allCorrect ? previousWords[previousWords.Count-1] : letters,exits,fontOnWhite);

        keyboardManager.EntryLetters(letters.ToUpper(),exits);

        //Debug.Log("All correct: ");
        //Debug.Log(allCorrect);
        if(allCorrect)
        {
            IncreaseScore();
            GenerateNewAnswer();
        }
        else
        {
            allTries++;
            tries++;
        }
        ClearGuess(isAllCorrect: allCorrect);

        UpdateText();
        currentGuess = "";
    }

    /*
    private void CheckPreviouslyGuessedLetters()
    {
        Transform child = transform.Find("Letter"+currentGuess.Length.ToString());
        if(child != null)
        {
            LetterControl letterControl = child.GetComponent<LetterControl>();
            if(letterControl.GetColor() == correct)
            {
                currentGuess += answerOfTurn.ToCharArray()[letterIndex];
                letterControl.SetLetter(answerOfTurn.ToCharArray()[letterIndex]);
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
                LetterControl letterControl = child.GetComponent<LetterControl>();
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
                LetterControl letterControl = child.GetComponent<LetterControl>();
                if (letterControl)
                {
                    letterControl.SetLetter(' ');
                    letterControl.isCorrect = false;
                    letterControl.SetColor(defaultColor);
                }
            }
            keyboardManager.ResetColors();
        }
    }

    private void IncreaseScore()
    {
        hits++;
        scorePoints += tries <= 0 ? 1000 : 1000/tries;
        tries = 0;
    }

    private void UpdateText()
    {
        score.text = "Pontos: " + scorePoints.ToString();
        allTriesText.text = "Tentativas: " + allTries.ToString();
        allCorrects.text = "Acertos: " + hits.ToString();
    }
}
