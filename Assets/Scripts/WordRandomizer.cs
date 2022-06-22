using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordRandomizer : MonoBehaviour
{
    public List<string> listPossibleAnswers = new List<string>();
    public List<string> listAllowedGuesses = new List<string>();
    private List<string> listAllowedGuessesNoAccents = new List<string>();

    private void Start()
    {
        listPossibleAnswers = ReadTextFile(Application.dataPath + "/StreamingAssets/words_answers.txt");
        listAllowedGuesses = ReadTextFile(Application.dataPath + "/StreamingAssets/words_broader.txt");
        listAllowedGuesses.ForEach((word) => listAllowedGuessesNoAccents.Add(TextManipulation.RemoveAccents(word).ToLower()));
    }

    public string GetRandomAnswer()
    {
        int wordIndex = Random.Range(0, listPossibleAnswers.Count);
        string randomWord = listPossibleAnswers[wordIndex];
        return randomWord.ToLower();
    }

    List<string> ReadTextFile(string file_path)
    {
        List<string> wordsFromFile = new List<string>();
        StreamReader inp_stm = new StreamReader(file_path);

        while(!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine( );
            wordsFromFile.Add(inp_ln.ToLower());
        }
        inp_stm.Close( );

        return wordsFromFile;
    }

    public bool IsInList(string word)
    {
        Debug.Log(listAllowedGuessesNoAccents.Contains(word));
        return listAllowedGuessesNoAccents.Contains(word);
    }
}
