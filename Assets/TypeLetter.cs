using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeLetter : MonoBehaviour
{

    private string invalidLetters = "1234567890-=_+*/.,;";

    public WordManager wordManager;
    void Update()
    {
        foreach(char letter in Input.inputString)
        {
            if(letter == '\b')
            {
                wordManager.DeleteLetter();
            }
            else if ((!invalidLetters.Contains(letter.ToString())) && letter!='\n' && letter!='\'')
            {
                wordManager.TypeLetter(letter);
                Debug.Log(letter);
            }
        }
        
    }
}
