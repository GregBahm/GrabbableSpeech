using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System;

public class MainScript : MonoBehaviour
{
    public Canvas Canvas;
    public Color InProgressTextColor;
    public TextMeshProUGUI OutputTextObj;

    public static MainScript Instance;
    private RectTransform canvasRect;
    
    public Vector2 VisibleCharactersCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvasRect = Canvas.GetComponent<RectTransform>();
        VisibleCharactersCount = GetVisibleCharactersCount();
    }

    public Vector2 GetVisibleCharactersCount()
    {
        float vertical = canvasRect.rect.width * OutputTextObj.fontSize * 40000;
        float horzontal = canvasRect.rect.width * OutputTextObj.fontSize * (13f / 7) * 10000;
        return new Vector2(vertical, horzontal);
    }

    void Update()
    {
        //OutputTextObj.text = GetFormattedLogText();
    }

    //private string GetFormattedLogText()
    //{
    //    string colorString = GetHexString(MainScript.Instance.InProgressTextColor);
    //    string speechInProgess = SpeechRecognitionManager.Instance.SpeechInProgress;
    //    string inProgressPart = "<color=" + colorString + ">" + speechInProgess + "</color>";
    //    return SpeechRecognitionManager.Instance.Log + inProgressPart;
    //}

    private static string GetHexString(Color color)
    {
        byte r = (byte)(byte.MaxValue * color.r);
        byte g = (byte)(byte.MaxValue * color.g);
        byte b = (byte)(byte.MaxValue * color.b);
        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }
}