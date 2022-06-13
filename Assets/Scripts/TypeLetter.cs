using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeLetter : MonoBehaviour
{

    //private string invalidLetters = "1234567890-=_+*/.,;";
    private string validLetters = "qwertyuiopasdfghjklçzxcvbnm";

    public WordManager wordManager;
    void Update()
    {
        foreach(char letter in Input.inputString.ToLower())
        {
            if(letter == '\b')
            {
                wordManager.DeleteLetter();
            }
            else if (letter == '\n' || letter == '\r')
            {
                wordManager.EnterWord();
            }
            else if (validLetters.Contains(letter.ToString()))
            {
                wordManager.TypeLetter(letter);
                //aDebug.Log(letter);
            }
        }
        
    }
}
