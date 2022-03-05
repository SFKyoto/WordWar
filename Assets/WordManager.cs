using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordManager : MonoBehaviour
{
    public AnswerManager answerManager;
    public IndicatorManager indicatorManager;
    public WordRandomizer wordRandomizer;
    public Text score;

    public string word = "cobra";
    public int letterIndex = 0;

    public Color correct;
    public Color miss;
    public Color incorrect;

    private int scorePoints = 0;

    private void Start() 
    {
        GenerateNewWord();
    }

    private void GenerateNewWord() 
    {
        word = wordRandomizer.NewWord();
    }

    public void TypeLetter(char letter)
    {
        foreach(Transform child in transform)
        {
            string letterIndexString = letterIndex.ToString();
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

    public void DeleteLetter()
    {
        if(letterIndex != 0)
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

    private void ReviseLetters()
    {
        string letters = "";
        Color[] exits = new Color[word.Length];
        foreach(Transform child in transform)
        {
            LetterControl script = child.GetComponent<LetterControl>();
            if (script)
            {
                letters += script.GetLetter();
            }
        }
        letters = letters.ToLower();
        int index1 = 0;
        foreach(char letter1 in letters)
        {
            int index2 = 0;
            bool found = false;
            foreach(char letter2 in word)
            {
                if(letter1 == letter2)
                {
                    if(index1 == index2)
                    {
                        exits[index1] = correct; //send green
                        found = true;
                        Transform child = transform.Find("Letter"+index1.ToString());
                        if(child != null)
                        {
                            LetterControl script = child.GetComponent<LetterControl>();
                            script.isCorrect = true;
                            script.SetColor(correct);
                        }
                        Debug.Log("Sim");
                        break;
                    }
                    else
                    {
                        exits[index1] = miss; //send yellow
                        found = true;
                        Debug.Log("Quase");
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

        answerManager.CreateAnswer(letters,exits);
        
        clear(0);

        bool allCorrect = true;

        Debug.Log(letters);

        foreach(Color exit in exits)
        {
            if(exit != correct) 
            {
                allCorrect = false;
            }
        }
        if(allCorrect)
        {
            IncreaseScore();
            GenerateNewWord();
            clear(1);
        }
    }

    private void IncreaseIndex()
    {
        letterIndex++;
        if(letterIndex >= 5)
        {
            letterIndex = 0;
            ReviseLetters();
            
        }
        Transform child = transform.Find("Letter"+letterIndex.ToString());
        if(child != null)
        {
            LetterControl script = child.GetComponent<LetterControl>();
            if(script.getColor() == correct)
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
                    script.SetColor(Color.white);
                }
            }
        }
    }

    private void DecreaseIndex()
    {
        letterIndex--;
    }

    private void IncreaseScore()
    {
        scorePoints += 100;
        score.text = "Pontos: " + scorePoints.ToString();
    }
}
