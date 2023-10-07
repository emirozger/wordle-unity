using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Key : MonoBehaviour
{
    [SerializeField]
    private KeyCode keyCode = KeyCode.None;
    private Image image = null;
    private Color startColor = Color.gray;
    private TextMeshProUGUI text;
    public Action<KeyCode> pressed;
    [Serializable]
    public struct LetterStateColor
    {
        public LetterState letterState;
        public Color color;
    }
    
    [SerializeField] private LetterStateColor[] letterStateColors = null;
    public KeyCode KeyCode { get {return keyCode;}}
    
    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text&&string.IsNullOrEmpty(text.text))
        {
            text.text = keyCode.ToString();
        }

        startColor = image.color;
    }

    private void Update()
    {
        switch (keyCode)
        {
            case KeyCode.Alpha0:
                text.text = 'Ğ'.ToString();
                break;
            case KeyCode.Alpha1:
                text.text = 'Ü'.ToString();
                break;
            case KeyCode.Alpha2:
                text.text = 'Ş'.ToString();
                break;
            case KeyCode.Alpha3:
                text.text = 'İ'.ToString();
                break;
            case KeyCode.Alpha4:
                text.text = 'Ö'.ToString();
                break;
            case KeyCode.Alpha5:
                text.text = 'Ç'.ToString();
                break;
        }
        
       
    }

    private void OnButtonClick()
    {
        pressed?.Invoke(keyCode);
    }
    public void SetState(LetterState letterState)
    {
        foreach (LetterStateColor letterStateColor in letterStateColors)
        {
            if (letterStateColor.letterState == letterState)
            {
                image.color = letterStateColor.color;
                break;
            }
        }    
    }

    public void ResetState()
    {
        image.color = startColor;
    }
}