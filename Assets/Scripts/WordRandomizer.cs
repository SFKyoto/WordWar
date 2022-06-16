using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordRandomizer : MonoBehaviour
{
    public List<string> words = new List<string>();
    public List<string> words_broader = new List<string>();
    private List<string> wordsSemAcentos = new List<string>();

    private void Start() 
    {
        words = ReadTextFile(Application.dataPath + "/StreamingAssets/words_answers.txt");
        words_broader = ReadTextFile(Application.dataPath + "/StreamingAssets/words_broader.txt");
        words.ForEach((word) => wordsSemAcentos.Add(word.ToLower()));
    }

    public string NewWord()
    {
        int wordIndex = Random.Range(0,words.Count);
        string word = words[wordIndex];
        return word.ToLower();
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
        Debug.Log(wordsSemAcentos.Contains(word));
        return wordsSemAcentos.Contains(word);
    }
}
