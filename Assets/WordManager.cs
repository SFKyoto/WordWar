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
        
        int index = 0;
        bool allCorrect = true;
        foreach(Transform child in transform)
        {
            LetterControl script = child.GetComponent<LetterControl>();
            if (script)
            {
                script.SetLetter(' ');
            }
        index++;
        }

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
            word = wordRandomizer.NewWord();
        }
    }

    private void IncreaseIndex()
    {
        letterIndex++;
        if(letterIndex >= 5)
        {
            letterIndex = 0;
            ReviseLetters();
            //clear letters
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
