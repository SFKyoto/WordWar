using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIKeyboardManager : MonoBehaviour
{
    public Color defaultColor;
    public Color correctColor;
    public Color missColor;
    public Color incorrectColor;
    public Color fontOnBlackColor;
    public Color fontOnWhiteColor;

    /// <summary>
    /// Colore uma letra do teclado.
    /// </summary>
    public void PaintKeyboardLetters(string letters, Color[] entry)
    {
        int index = 0;
        foreach (char letter in letters)
        {
            Transform child = transform.Find(letter.ToString());
            if(child != null)
            {
                GUIGuessedWordManager letraTeclado = child.GetComponent<GUIGuessedWordManager>();
                Color letterColor = letraTeclado.GetColor();
                if(letterColor != correctColor || (letterColor == missColor && entry[index] == correctColor))
                {
                    letraTeclado.SetBgColor(entry[index]);
                    letraTeclado.SetFontColor(fontOnWhiteColor);
                }
            }
            index++;
        }
    }

    /// <summary>
    /// Reseta as cores das letras do teclado para a inicial.
    /// </summary>
    public void ResetColors()
    {
        foreach(Transform child in transform)
        {
            if(child != null)
            {
                GUIGuessedWordManager guiGessedWordManager = child.GetComponent<GUIGuessedWordManager>();
                guiGessedWordManager.SetFontColor(fontOnBlackColor);
                guiGessedWordManager.SetBgColor(defaultColor);
            }
        }
    }
}
