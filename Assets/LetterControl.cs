using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterControl : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Text text;
    private Color status;
    public bool isCorrect = false;

    public void SetLetter(char letter)
    {
        text.text = letter.ToString().ToUpper();
    }

    public void Correct()
    {
        if(!isCorrect) particleSystem.Play();
        isCorrect = true;
    }

    public char GetLetter()
    {
        return text.text.ToCharArray()[0];
    }

    public void SetColor(Color _status)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = _status;
        status = _status;
    }

    public Color getColor()
    {
        return status;
    }
}
