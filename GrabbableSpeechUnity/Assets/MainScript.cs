using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System;

public class MainScript : MonoBehaviour
{
    public Transform SelectionStart;
    public Transform SelectionEnd;

    public ISpeechSource SpeechSource { get; private set; }
    public Canvas Canvas;
    public TextMeshProUGUI LogTextObj;
    public TextMeshProUGUI InProgressTextObj;
    public bool AllowLogChanges { get; set; }

    public static MainScript Instance;
    private RectTransform canvasRect;
    private RectTransform speechLogRect;
    private StringBuilder testTextBuilder = new StringBuilder();

    public Vector2 VisibleCharactersCount { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvasRect = Canvas.GetComponent<RectTransform>();
        speechLogRect = LogTextObj.GetComponent<RectTransform>(); 
        VisibleCharactersCount = GetVisibleCharactersCount();

        SpeechSource = SpeechRecognitionManager.Instance.isActiveAndEnabled ? (ISpeechSource)SpeechRecognitionManager.Instance : FakeSpeechGenerator.Instance;
    }

    public Vector2 GetVisibleCharactersCount()
    {
        float vertical = canvasRect.rect.width * LogTextObj.fontSize * 10000;
        float horzontal = canvasRect.rect.width * LogTextObj.fontSize * (13f / 7) * 2500;
        return new Vector2(vertical, horzontal);
    }

    void Update()
    {
        LogTextObj.text = LineSelector.Instance.GetLogText();
        InProgressTextObj.text = SpeechSource.SpeechInProgress;
        speechLogRect.offsetMin = new Vector2(0, ScrollingManager.Instance.Scrollage);
    }

    private string GetLogTextBasic()
    {
        testTextBuilder.Clear();
        foreach (SpeechBlock block in MainScript.Instance.SpeechSource.Blocks)
        {
            foreach (string line in block.Lines)
            {
                testTextBuilder.AppendLine(line);
            }
        }
        return testTextBuilder.ToString();
    }

    private static string GetHexString(Color color)
    {
        byte r = (byte)(byte.MaxValue * color.r);
        byte g = (byte)(byte.MaxValue * color.g);
        byte b = (byte)(byte.MaxValue * color.b);
        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }
}