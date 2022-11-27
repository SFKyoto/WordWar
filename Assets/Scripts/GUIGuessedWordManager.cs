using UnityEngine;
using UnityEngine.UI;

public class GUIGuessedWordManager : MonoBehaviour
{
    //public ParticleSystem particleSystem;
    public Text letter;
    private Color bgColor;
    public bool isCorrect = false;

    public void SetLetter(char letter)
    {
        //Debug.Log(letter);
        this.letter.text = letter.ToString().ToUpper();
    }

    public void Correct()
    {
        //if(!isCorrect) particleSystem.Play();
        isCorrect = true;
    }

    public char GetLetter()
    {
        return letter.text.ToCharArray()[0];
    }

    public void SetBgColor(Color _status)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = _status;
        bgColor = _status;
    }

    public Color GetColor()
    {
        return bgColor;
    }

    public void SetFontColor(Color fontColor)
    {
        letter.color = fontColor;
    }

    public void HideComponent()
    {
        letter.color = new Color(0, 0, 0, 0);
        GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
    }
}
