using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public const int wordLenght = 5;
    [SerializeField] Letter letterPrefab = null;
    [SerializeField] int amountOfRows = 6;
    [SerializeField] GridLayoutGroup gridLayout=null;
    [SerializeField] WordListManager wordListManager=null;
    [SerializeField] private Key[] keys;
    int index = 0;
    int currentRow = 0;
    float letterAnimWaitTime = .35f;
    char?[] guess  = new char?[wordLenght];
    char[] word = new char[wordLenght];
    [SerializeField] private GameObject wordNotFound;
    [SerializeField] private TextMeshProUGUI wonText;
    List<Letter> letters = null;
    public GameState gameState { get; private set; } = GameState.InProgress;
    private Action restarted;
  

    private void Awake()
    {
        SetupGrid();
        foreach (Key key in keys)
        {
            key.pressed += OnKeyPressed;
        }
    }
    void Start()
    {
        SetWord();
    }

    void Update()
    {
        
        if (Input.anyKeyDown)
        { 
            ParseInput(Input.inputString);
        }
        
    }

    public void Restart()
    {
        wonText.gameObject.SetActive(false);
        gameState = GameState.InProgress;
        foreach (Letter letter in letters)
        {
            letter.Clear();
        }

        foreach (Key key in keys)
        {
            key.ResetState();
        }
        index = 0;
        currentRow = 0;
        
        for (int i = 0; i < wordLenght; i++)
        {
            guess[i] = null;
        }
        SetWord();

        restarted?.Invoke();
    }

    void OnKeyPressed(KeyCode keyCode)
    {
        if (gameState!=GameState.InProgress)
        {
            if (keyCode==KeyCode.Return)
            {
                Restart();
            }
            return;
        }

        if (keyCode==KeyCode.Return)
        {
            GuessWord();
        }
        else if (keyCode == KeyCode.Backspace || keyCode == KeyCode.Delete)
        {
            DeleteLetter();
        }
        else if (keyCode == KeyCode.Alpha0)
        {
            EnterLetter('Ğ');
        }
        else if (keyCode == KeyCode.Alpha1)
        {
            EnterLetter('Ü');
        }
        else if (keyCode == KeyCode.Alpha2)
        {
            EnterLetter('Ş');
        }
        else if (keyCode == KeyCode.Alpha3)
        {
            EnterLetter('İ');
        }
        else if (keyCode == KeyCode.Alpha4)
        {
            EnterLetter('Ö');
        }
        else if (keyCode == KeyCode.Alpha5)
        {
            EnterLetter('Ç');
        }
        else
        {
            var index = keyCode - KeyCode.A;
            Debug.Log(index);
            EnterLetter((char)((int)'A' + index));
        }
       
    }
    

    public void Cheat()
    {
        if (gameState!=GameState.InProgress)
            return;

        string word = null;

        if (currentRow>0)
        {
            word = wordListManager.GetWord(letters.GetRange(0,currentRow*wordLenght).ToArray());
        }

        if (string.IsNullOrWhiteSpace(word))
        {
            word = wordListManager.GetRandomWord();
        }
        ParseInput(word);
        OnKeyPressed(KeyCode.Return);
        
    }

    public void ParseInput(string value)
    {
        if (gameState != GameState.InProgress)
        {
            foreach (char c in value)
            {
                if ((c== '\n') || (c=='\r')) //enter or return
                {
                    Restart();
                    return;
                }
            }
            return;
        }
            

        foreach (char c in value)
        {
            if (c == '\b')  
            {
                DeleteLetter();
            }
            else if ((c== '\n') || (c=='\r'))
            {
                GuessWord();
            }
            else 
            {
                EnterLetter(c);
            }
        }
    }
    public void SetupGrid()
    {
        if (letters==null)
            letters = new List<Letter>();

        for (int i = 0; i < amountOfRows; i++)
        {
            for (int j = 0; j < wordLenght; j++)
            {
                Letter letter = Instantiate<Letter>(letterPrefab);
                letter.transform.SetParent(gridLayout.transform);
                letters.Add(letter);
            }
           
        }
    }
    public void SetWord()
    {
        string randomWord = wordListManager.GetRandomWord();
        
        for (int i = 0; i < randomWord.Length; i++)
        {
            this.word[i] = randomWord[i];
            word[i] = char.ToUpper(word[i]);     
        }
    }
    
    public void EnterLetter(char c)
    {
        if (index<wordLenght)
        {
            c = char.ToUpper(c);
            letters[(currentRow*wordLenght)+index].EnterLetter(c);
            guess[index]=c;
            index++;
        }
    }

    public void DeleteLetter()
    {
        if (index>0)
        {
            index--;
            letters[(currentRow * wordLenght) + index].DeleteLetter();
            guess[index]=null;
        }
    }
    public void GuessWord()
    {
        if (index!=wordLenght)
        {
            Shake();
        }
        else
        {
            
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < wordLenght; i++)
            {
                sb.Append(guess[i].Value);
                
            }

            Debug.Log(sb.ToString());
            
            if (wordListManager.CheckWordExists(sb.ToString()))
            {
                
                bool incorrect = false;

                for (int i = 0; i < wordLenght; i++)
                {
                    bool correct = this.guess[i] == this.word[i];

                    if (!correct)
                    {
                        incorrect = true;
                        
                        bool letterExistsInWorld = false;
                        for (int j = 0; j < wordLenght; j++)
                        {
                            letterExistsInWorld = this.guess[i] == this.word[j];
                            if (letterExistsInWorld)                           
                                break;                           
                        }
                        StartCoroutine(PlayLetter(i * letterAnimWaitTime, (currentRow * wordLenght) + i, letterExistsInWorld ? LetterState.WrongLocation : LetterState.Incorrect));
                    }
                    else
                    {
                        StartCoroutine(PlayLetter(i * letterAnimWaitTime, (currentRow * wordLenght) + i, LetterState.Correct));
                    }
                }
                if (incorrect)
                {
                    index = 0;
                    currentRow++;
                    if (currentRow>=amountOfRows)
                    {
                        gameState = GameState.Failed;
                    }

                }
                else
                {
                    gameState = GameState.Complete;
                    wonText.gameObject.SetActive(true);
                }
            }
            else
            {
                //5 kelime girdiginde wordlistte yoksa shakeliyom burası.
                StartCoroutine(WordNotFound(wordNotFound, .5f));
            }
        }
    }
    IEnumerator PlayLetter(float value, int index, LetterState state)
    {
        yield return new WaitForSeconds(value);
        letters[index].SetState(state);

        int indexOfChar = (int)letters[index].Entry.Value - (int)'A';
        KeyCode keyCode = indexOfChar + KeyCode.A;
        
        foreach (Key key in keys)
        {
            if (key.KeyCode==keyCode)
            {
                key.SetState(state);
                break;
            }    
        }
    }

    IEnumerator WordNotFound(GameObject obj,float delay)
    {
        obj.gameObject.SetActive(true);
        Shake();
        yield return new WaitForSeconds(delay);
        obj.gameObject.SetActive(false);
    }
    public void Shake()
    {
        for (int i = 0; i < wordLenght; i++)
        {
            letters[(currentRow * wordLenght) + i].Shake();
        }
    }
}