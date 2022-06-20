using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    public Color defaultColor;
    public Color correct;
    public Color miss;
    public Color incorrect;
    public Color fontOnBlack;
    public Color fontOnWhite;

    public void EntryLetters(string letters, Color[] entry)
    {
        int index = 0;
        foreach (char letter in letters)
        {
            Transform child = transform.Find(letter.ToString());
            if(child != null)
            {
                LetterControl script = child.GetComponent<LetterControl>();
                Color letterColor = script.GetColor();
                if(letterColor!=correct || (letterColor==miss && entry[index]==correct))
                {
                    script.SetColor(entry[index]);
                    script.SetFontColor(fontOnWhite);
                }
            }
            index++;
        }
    }

    public void ResetColors()
    {
        foreach(Transform child in transform)
        {
            if(child != null)
            {
                LetterControl script = child.GetComponent<LetterControl>();
                script.SetFontColor(fontOnBlack);
                script.SetColor(defaultColor);
            }
        }
    }
}
