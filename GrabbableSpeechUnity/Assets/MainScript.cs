using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System;

public class MainScript : MonoBehaviour
{
    public ISpeechSource SpeechSource { get; private set; }
    public Canvas Canvas;
    public TextMeshProUGUI LogTextObj;
    public TextMeshProUGUI InProgressTextObj;

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

        SpeechSource = SpeechRecognitionManager.Instance;
        //SpeechSource = new FakeSpeechGenerator();
    }

    public Vector2 GetVisibleCharactersCount()
    {
        float vertical = canvasRect.rect.width * LogTextObj.fontSize * 40000;
        float horzontal = canvasRect.rect.width * LogTextObj.fontSize * (13f / 7) * 10000;
        return new Vector2(vertical, horzontal);
    }

    void Update()
    {
        LogTextObj.text = TextSelection.Instance.GetLogText();
        InProgressTextObj.text = SpeechSource.SpeechInProgress;
    }

    private static string GetHexString(Color color)
    {
        byte r = (byte)(byte.MaxValue * color.r);
        byte g = (byte)(byte.MaxValue * color.g);
        byte b = (byte)(byte.MaxValue * color.b);
        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }
}