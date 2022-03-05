using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordRandomizer : MonoBehaviour
{
    public List<string> words = new List<string>();

    private void Start() 
    {
        ReadTextFile(Application.dataPath + "/StreamingAssets/words.txt");
    }

    public string NewWord()
    {
        int wordIndex = Random.Range(0,words.Count);
        string word = words[wordIndex];
        return word.ToLower();
    }

    void ReadTextFile(string file_path)
    {
        StreamReader inp_stm = new StreamReader(file_path);

        while(!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine( );
            words.Add(TextManipulation.RemoveAccents(inp_ln));
        }

        inp_stm.Close( );  
    }
}
