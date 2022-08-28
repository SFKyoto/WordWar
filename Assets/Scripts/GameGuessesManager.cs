using UnityEngine;

public abstract class GameGuessesManager : MonoBehaviour
{
    public abstract string GetCheckedAttempt(string currentGuess);

}
