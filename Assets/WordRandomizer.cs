using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordRandomizer : MonoBehaviour
{
    public string[] words = {"cobra","carro","corra","monta","tonta"};

    public string NewWord()
    {
        int wordIndex = Random.Range(0,words.Length);
        string word = words[wordIndex];
        return word;
    }
}
