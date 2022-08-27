using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SinglePlayerWordsManager : MonoBehaviour
{
    public List<string> listPossibleAnswers = new List<string>();
    public List<string> listAllowedGuesses = new List<string>();
    private List<string> listAllowedGuessesNoAccents = new List<string>();

    /// <summary>
    /// Obtém as listas de tentativas e repostas possíveis de arquivos incluídos com o jogo.
    /// </summary>
    private void GetWordLists()
    {
        listPossibleAnswers = ReadTextFile(Application.dataPath + "/StreamingAssets/words_answers.txt");
        listAllowedGuesses = ReadTextFile(Application.dataPath + "/StreamingAssets/words_broader.txt");
        listAllowedGuesses.ForEach((word) => listAllowedGuessesNoAccents.Add(SinglePlayerTextManipulation.RemoveAccents(word).ToLower()));
    }

    /// <summary>
    /// Retorna uma palavra aleatória da lista de possíveis tentativas.
    /// </summary>
    public string GetRandomAnswer()
    {
        if (listPossibleAnswers.Count == 0) GetWordLists();
        int wordIndex = Random.Range(0, listPossibleAnswers.Count);
        string randomWord = listPossibleAnswers[wordIndex];
        return randomWord.ToLower();
    }

    /// <summary>
    /// Lê um arquivo do sistema com uma lista de palavras e a retorna como objeto List.
    /// </summary>
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

    /// <summary>
    /// Checa se uma palavra é válida na lista de tentativas possíveis.
    /// </summary>
    public bool IsInList(string word)
    {
        Debug.Log(listAllowedGuessesNoAccents.Contains(word));
        return listAllowedGuessesNoAccents.Contains(word);
    }
}
