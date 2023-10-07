
using TMPro;
using UnityEngine;

public class Letter : MonoBehaviour
{
    private readonly int animatorResetTrigger = Animator.StringToHash("Reset");
    private readonly int animatorShakeTrigger = Animator.StringToHash("Shake");
    private readonly int animatorStateParameter = Animator.StringToHash("State");

    public LetterState LetterState { get; private set; } = LetterState.Blank;
    
    private Animator animator = null;
    public TMP_Text text = null;
    
    public char? Entry { get; private set; } = null;
    void Awake()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        text.text = null;
    }

    public void EnterLetter(char c)
    {
        Entry = c;
        text.text = c.ToString().ToUpper();
    }

    public void DeleteLetter()
    {
        Entry = null;
        text.text = null;
        animator.SetTrigger(animatorResetTrigger);
    }
    public void Shake()
    {
        animator.SetTrigger(animatorShakeTrigger);
    }

    public void Clear()
    {
        LetterState = LetterState.Blank;
        animator.SetInteger(animatorStateParameter,-1);
        animator.SetTrigger(animatorResetTrigger);
        Entry = null;
        text.text = null;
    }

    public void SetState(LetterState state)
    {
        LetterState = state;
        animator.SetInteger(animatorStateParameter,(int)state);
    }
}