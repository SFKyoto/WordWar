using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    public GameObject letterSquare;

    private int answerIndex = 0;

    public float animationTime = 1.3f;

    public void CreateAnswer(string answer, Color[] colors, Color fontColor)
    {
        for(int i=0;i<answer.Length;i++)
        {
            //GameObject newLetterSquare = Instantiate(letterSquare) as GameObject;
            //newLetterSquare.transform(new Vector3((i*1.25f)-2.5f,2.25f-answerIndex*1.25f,0));
            GameObject newLetterSquare = Instantiate(letterSquare,new Vector3((i*1.25f)-2.5f,0.5f,0),Quaternion.identity, transform) as GameObject;
            LetterControl script = newLetterSquare.GetComponent<LetterControl>();
            if(script)
            {
                script.SetLetter(answer.ToCharArray()[i]);
                script.SetColor(colors[i]);
                script.SetFontColor(fontColor);
            } 
        }
        MoveAnswers();
        answerIndex++;
    }

    /*Move respostas para o topo da tela.*/
    public void MoveAnswers()
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
