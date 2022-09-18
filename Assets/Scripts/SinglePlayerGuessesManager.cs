using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SinglePlayerGuessesManager : GameGuessesManager
{
    private void Start()
    {
        GenerateNewAnswer();
    }

    [Header("Word Lists")]
    public List<string> listPossibleAnswers = new List<string>();
    public List<string> listAllowedGuesses = new List<string>();
    private List<string> listAllowedGuessesNoAccents = new List<string>();

    [Header("Round State")]
    public string answerOfTurn = "cobra";
    public string answerOfTurnNoAccents;

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
    private string GetRandomAnswer()
    {
        if (listPossibleAnswers.Count == 0) GetWordLists();
        int wordIndex = Random.Range(0, listPossibleAnswers.Count);
        string randomWord = listPossibleAnswers[wordIndex];
        return randomWord.ToLower();
    }

    /// <summary>
    /// Lê um arquivo do sistema com uma lista de palavras e a retorna como objeto List.
    /// </summary>
    private List<string> ReadTextFile(string file_path)
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
    private bool IsInGuessList(string word)
    {
        Debug.Log(listAllowedGuessesNoAccents.Contains(word));
        return listAllowedGuessesNoAccents.Contains(word);
    }

    private void GenerateNewAnswer()
    {
        answerOfTurn = GetRandomAnswer();
        answerOfTurnNoAccents = SinglePlayerTextManipulation.RemoveAccents(answerOfTurn);
    }

    /// <summary>
    /// Compara tentativa com palavra da rodada e retorna uma cadeia de caracteres com dicas da palavra correta;<br>
    /// 'C' para letras na posição correta, 'M' para letras fora da posição da palavra, 'E' para letras que não estão na palavra.</br>
    /// Retorna "X" se a palavra não está na lista de tentativas possíveis.
    /// </summary>
    public override string GetCheckedAttempt(string currentGuess)
    {
        if (!IsInGuessList(currentGuess))
        {
            return "X";
        }

        char[] returnedAnswer = new char[5];
        string currentGuessNoAccents = SinglePlayerTextManipulation.RemoveAccents(currentGuess);
        string answerCopy = answerOfTurnNoAccents;
        int indexWordCopy = 0;
        bool allCorrect = true;

        //primeiro checamos quais letras são corretas
        Debug.Log(currentGuessNoAccents);
        Debug.Log(answerCopy);
        for (int i = 0; i < currentGuessNoAccents.Length; i++)
        {
            allCorrect = allCorrect && (currentGuessNoAccents[i] == answerOfTurnNoAccents[i]);

            if (currentGuessNoAccents[i] == answerOfTurnNoAccents[i])
            {
                returnedAnswer[i] = 'C';
                answerCopy = answerCopy.Remove(indexWordCopy, 1);
                Debug.Log("Correto na pos " + i.ToString());
                indexWordCopy--;
                Debug.Log("wordCopy agora: " + answerCopy);
            }
            indexWordCopy++;
        }

        //answerCopy só tem as letras incorretas agora
        //ex: MYLLA + tentativa LLLAL = MYLA
        //vamos checar quais ganham cor 'amarela' ou 'cinza'
        Debug.Log("Palavra que sobrou: " + answerCopy);
        for (int i = 0; i < currentGuessNoAccents.Length; i++)
        {
            if (currentGuessNoAccents[i] != answerOfTurnNoAccents[i])
            {
                int indexLetraQuase = answerCopy.IndexOf(currentGuessNoAccents[i]);
                if (indexLetraQuase != -1)
                {
                    returnedAnswer[i] = 'M';
                    Debug.Log("Quase na pos " + i.ToString());
                    answerCopy = answerCopy.Remove(indexLetraQuase, 1);
                }
                else
                {
                    returnedAnswer[i] = 'I';
                    Debug.Log("Erradissimo na pos " + i.ToString());
                }
            }
        }

        if (allCorrect)
        {
            GenerateNewAnswer();
        }

        return new string(returnedAnswer);
    }

}
