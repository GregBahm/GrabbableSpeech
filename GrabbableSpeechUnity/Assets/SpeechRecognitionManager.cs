using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System;

public class SpeechRecognitionManager : MonoBehaviour
{
    private readonly List<SpeechBlock> speechBlocks = new List<SpeechBlock>();
    public string SpeechInProgress { get; private set; }
    public string Log { get; private set; }

    private SpeechRecognizer recognizer;
    private readonly object threadLocker = new object();

    public static SpeechRecognitionManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartRecognizer();
    }

    private void StartRecognizer()
    {
        SpeechConfig config = SpeechConfig.FromSubscription(SpeechSubscriptionKey.Key, "westus");
        recognizer = new SpeechRecognizer(config);
        recognizer.Recognizing += Recognizer_Recognizing;
        //recognizer.Recognized += Recognizer_Recognized;
        recognizer.StartContinuousRecognitionAsync();
    }

    private void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
    {
        Debug.Log(e);
        SpeechBlock newBlock = new SpeechBlock(e.Result.Text, DateTime.Now);
        lock (threadLocker)
        {
            SpeechInProgress = "";
            speechBlocks.Add(newBlock);
            Log = UpdateLog();
        }
    }

    private string UpdateLog()
    {
        StringBuilder logBuilder = new StringBuilder();
        foreach (SpeechBlock item in speechBlocks)
        {
            logBuilder.AppendLine(item.Text);
        }
        return logBuilder.ToString();
    }

    private void Recognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            Debug.Log(e);
            //SpeechInProgress = e.Result.Text;
        }
    }

    private void OnDestroy()
    {
        if(recognizer != null)
        {
            recognizer.Dispose();
        }
    }
}

public class SpeechBlock
{
    public DateTime TimeStamp { get; }
    public string Text { get; }

    public SpeechBlock(string text, DateTime timeStamp)
    {
        Text = text;
        TimeStamp = timeStamp;
    }
}