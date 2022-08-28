using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    public GameObject letterSquare;
    public float animationTime;

    /// <summary>
    /// Checa a resposta da rodada com a palavra tentada pelo usuário, e se for correta move as respostas para cima.
    /// </summary>
    public void CreateAnswerSprites(string answer, Color[] attempBgColors, Color fontColor)
    {
        for(int i=0;i<answer.Length;i++)
        {
            GameObject newLetterSquare = Instantiate(letterSquare,new Vector3((i*1.25f)-2.5f,0.5f,0),Quaternion.identity, transform) as GameObject;
            GUIGuessedWordManager guiGuessLetter = newLetterSquare.GetComponent<GUIGuessedWordManager>();
            if(guiGuessLetter)
            {
                guiGuessLetter.SetLetter(answer.ToCharArray()[i]);
                guiGuessLetter.SetBgColor(attempBgColors[i]);
                guiGuessLetter.SetFontColor(fontColor);
            } 
        }
        MoveAnswersUp();
    }

    /// <summary>
    /// Move respostas anteriores para o topo da tela.
    /// </summary>
    public void MoveAnswersUp()
    {
        foreach(Transform child in transform)
        {
            iTween.MoveTo(child.gameObject,child.position + new Vector3(0,1.25f,0),animationTime);
            //child.position = child.position + new Vector3(0,-1.25f,0);
            if(child.position.y > 6)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
