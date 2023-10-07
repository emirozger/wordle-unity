using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WordListManager : MonoBehaviour
{
    [SerializeField] TextAsset wordList = null;
    List<string> words = null;

    private void Awake()
    {
        words=new List<string>(wordList.text.Split(new char[] {',' , ' ' , '\n' , '\r' },System.StringSplitOptions.RemoveEmptyEntries));
    }

    public string GetRandomWord()
    {
        var randomWord = words[Random.Range(0, words.Count)];
        randomWord.ToUpper();
        return randomWord;
    }
    public bool CheckWordExists(string word) 
    {
        return words.Contains(word);
    }

    public string GetWord(Letter[] letters)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < letters.Length; i++)
        {
            if (letters[i].LetterState==LetterState.Correct || letters[i].LetterState == LetterState.WrongLocation)
            {
                sb.Append(letters[i].Entry.Value);
            }
        }

        List<string> answers = new List<string>();
        foreach (string word in words.Where(w =>sb.ToString().All(w.Contains)))
        {
            bool correct = true;
            for (int j = 0; j < letters.Length; j+=GameManager.wordLenght)
            {
                for (int i = 0; i < GameManager.wordLenght; i++)
                {
                    var letterIndex = j + i;
                    if (letters[letterIndex].LetterState==LetterState.Blank)
                        continue;
                    var letterMatches = word[i] == letters[letterIndex].Entry.Value;
                    
                    if ((letters[letterIndex].LetterState ==LetterState.Incorrect||letters[letterIndex].LetterState ==LetterState.WrongLocation)&& letterMatches)
                    {
                        correct = false;
                        break;
                    }
                    else
                    if (letters[letterIndex].LetterState==LetterState.Correct && !letterMatches)
                    {
                        correct = false;
                        break;
                    }   
                   
                }
                if (!correct)
                    break;
            }
            
            if (correct)
            {
                answers.Add(word);
            }
        }

        if (answers.Count>0)
        {
            return answers[Random.Range(0, answers.Count - 1)];
        }
        return null;
    }
}
