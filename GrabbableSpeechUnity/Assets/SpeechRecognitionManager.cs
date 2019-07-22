using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class SpeechRecognitionManager : MonoBehaviour, ISpeechSource
{
    private readonly ConcurrentQueue<SpeechBlock> blocks = new ConcurrentQueue<SpeechBlock>();
    public IEnumerable<SpeechBlock> Blocks { get { return blocks; } }
    public string SpeechInProgress { get; private set; }

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
        recognizer.Recognized += Recognizer_Recognized;
        recognizer.Canceled += Recognizer_Canceled;
        recognizer.StartContinuousRecognitionAsync();
    }

    private void Recognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
    {
        SpeechInProgress = e.ErrorDetails;
    }

    private void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
    {
        int charactersPerLine = Mathf.FloorToInt(MainScript.Instance.VisibleCharactersCount.x);
        SpeechBlock newBlock = new SpeechBlock(e.Result.Text, DateTime.Now, charactersPerLine);
        blocks.Enqueue(newBlock);
        lock (threadLocker)
        {
            SpeechInProgress = "";
        }
    }

    private string UpdateLog()
    {
        StringBuilder logBuilder = new StringBuilder();
        foreach (SpeechBlock item in blocks)
        {
            logBuilder.AppendLine(item.BaseText);
        }
        return logBuilder.ToString();
    }

    private void Recognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            SpeechInProgress = e.Result.Text;
        }
    }

    private async void OnDestroy()
    {
        if(recognizer != null)
        {
            await recognizer.StopContinuousRecognitionAsync();
            recognizer.Dispose();
        }
    }
}
