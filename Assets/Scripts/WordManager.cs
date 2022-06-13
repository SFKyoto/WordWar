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
    public Text score;
    public Text allTriesText;
    public Text allCorrects;

    public string word = "cobra";
    public List<string> previousWords = new List<string>();
    public int letterIndex = 0;

    [Header("Cores")]
    public Color defaultColor;
    public Color correct;
    public Color miss;
    public Color incorrect;
    
    public Color fontOnBlack;
    public Color fontOnWhite;

    private int scorePoints = 0;
    private int allTries = 0;
    private int tries = 0;
    private int hits = 0;
    
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

    private void GenerateNewWord() 
    {
        string newWord = wordRandomizer.NewWord();
        previousWords.Add(newWord);
        word = TextManipulation.RemoveAccents(newWord);
        exits = new Color[word.Length];
    }

    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        GenerateNewWord();
    }

    public void TypeLetter(char letter)
    {
        if(letterIndex < 5)
        {
            foreach(Transform child in transform)
            {
                string letterIndexString = letterIndex.ToString();
                //Debug.Log("letterIndexString: "+letterIndexString);
                //Debug.Log("child.name: "+child.name);
                if (child.name == "Letter"+letterIndexString)
                {
                    LetterControl script = child.GetComponent<LetterControl>();
                    if (script)
                    {
                        script.SetLetter(letter);
                    } 
                }
            }
            IncreaseIndex();
            indicatorManager.MoveIndicator(letterIndex*1.25f);
        }
    }

    public void DeleteLetter()
    {
        if(letterIndex > 0)
        {
            DecreaseIndex();
            foreach(Transform child in transform)
            {
                string letterIndexString = letterIndex.ToString();
                if (child.name == "Letter"+letterIndexString)
                {
                    LetterControl script = child.GetComponent<LetterControl>();
                    if (script)
                    {
                        script.SetLetter(' ');
                    } 
                }
            }
            indicatorManager.MoveIndicator(letterIndex*1.25f);
        }
    }

    public void EnterWord()
    {
        if(letterIndex==5 && wordRandomizer.IsInList(GetWord()))
        {
            letterIndex = 0;
            ReviseLetters();
        }
    }

    private string GetWord()
    {
        string letters = "";
        foreach(Transform child in transform)
        {
            LetterControl script = child.GetComponent<LetterControl>();
            if (script)
            {
                letters += script.GetLetter();
            }
        }
        letters = letters.ToLower();
        return letters;
    }

    private void ReviseLetters()
    {
        string letters = GetWord();
        //StringBuilder wordCopy = new StringBuilder(word);
        string wordCopy = word;
        int indexWordCopy = 0;

        //primeiro checamos quais letras s�o corretas
        for(int i = 0; i < letters.Length; i++){
            if(letters[i] == word[i]){
                exits[i] = correct;
                //wordCopy[i] = '0';
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
            if(letters[i] != word[i]){
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


        /*
        foreach(char letter1 in letters)
        {
            if(letter1 )
            int index2 = 0;
            bool found = false;
            foreach(char letter2 in word)
            {
                if(letter1 == letter2)
                {
                    found = true;
                    if(exits[index2] != correct)
                    {
                        if(index1 == index2)
                        {
                            exits[index1] = correct; //send green
                            /*
                            Transform child = transform.Find("Letter"+index1.ToString());
                            if(child != null)
                            {
                                LetterControl script = child.GetComponent<LetterControl>();
                                script.Correct();
                                script.SetColor(correct);
                            }
                            *-/
                            Debug.Log("Sim");
                            break;
                        }
                        else
                        {
                            exits[index1] = miss; //send yellow
                            Debug.Log("Quase");
                        }
                    }
                    else if(exits[index1] == correct) 
                    {
                        exits[index1] = incorrect;
                        break;
                    }
                }
                index2++;
            }
            if (!found)
            {
                exits[index1] = incorrect; //send red
                Debug.Log("Nao");
            }
        index1++;
        }
        */
        clear(0);

        bool allCorrect = true;

        Debug.Log(letters);

        foreach(Color exit in exits)
        {
            if(exit != correct) 
            {
                allCorrect = false;
            }
            //Debug.Log(exit);
        }

        if(allCorrect)
        {
            int x = previousWords.Count;
            answerManager.CreateAnswer(previousWords[x-1],exits,fontOnWhite);
        }
        else
        {
            answerManager.CreateAnswer(letters,exits,fontOnWhite);
        }
        keyboardManager.EntryLetters(letters.ToUpper(),exits);

        //Debug.Log("All correct: ");
        //Debug.Log(allCorrect);
        if(allCorrect)
        {
            IncreaseScore();
            GenerateNewWord();
            clear(1);
        }
        else
        {
            allTries++;
            tries++;
        }

        UpdateText();
    }

    private void IncreaseIndex()
    {
        letterIndex++;
        Transform child = transform.Find("Letter"+letterIndex.ToString());
        if(child != null)
        {
            LetterControl script = child.GetComponent<LetterControl>();
            if(script.GetColor() == correct)
            {
                script.SetLetter(word.ToCharArray()[letterIndex]);
                IncreaseIndex();
            }
        }
    }

    private void clear(int code)
    {
        if(code == 0)
        {
            foreach(Transform child in transform)
            {
                LetterControl script = child.GetComponent<LetterControl>();
                if (script)
                {
                    if(!script.isCorrect)
                    {
                        script.SetLetter(' ');
                    }
                }
            }
        }
        else if(code == 1)
        {
            foreach(Transform child in transform)
            {
                LetterControl script = child.GetComponent<LetterControl>();
                if (script)
                {
                    script.SetLetter(' ');
                    script.isCorrect = false;
                    script.SetColor(defaultColor);
                }
            }
            keyboardManager.Reset();
        }
    }

    private void DecreaseIndex()
    {
        letterIndex--;
    }

    private void IncreaseScore()
    {
        hits++;
        if(tries != 0) scorePoints += 1000/tries;
        else scorePoints += 1000;
        tries = 0;
    }

    private void UpdateText()
    {
        score.text = "Pontos: " + scorePoints.ToString();
        allTriesText.text = "Tentativas: " + allTries.ToString();
        allCorrects.text = "Acertos: " + hits.ToString();
    }
}
