using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterControl : MonoBehaviour
{
    public Text text;

    public void SetLetter(char letter)
    {
        text.text = letter.ToString().ToUpper();
    }

    public char GetLetter()
    {
        return text.text.ToCharArray()[0];
    }

    public void SetColor(Color status)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = status;
    }
}
