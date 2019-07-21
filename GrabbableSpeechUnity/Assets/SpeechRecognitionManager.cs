using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using TMPro;
using System.Text;

public class SpeechRecognitionManager : MonoBehaviour
{
    public Color InProgressTextColor;
    public TextMeshPro outputText;
    private StringBuilder messageLog = new StringBuilder();
    private string currentMessage;

    private SpeechRecognizer recognizer;
    private object threadLocker = new object();

    private void Start()
    {
        SpeechConfig config = SpeechConfig.FromSubscription(SpeechSubscriptionKey.Key, "westus");
        recognizer = new SpeechRecognizer(config);
        recognizer.SpeechStartDetected += Recognizer_SpeechStartDetected;
        recognizer.Recognizing += Recognizer_Recognizing;
        recognizer.Recognized += Recognizer_Recognized;
        recognizer.StartContinuousRecognitionAsync();
    }

    private static string GetHexString(Color color)
    {
        byte r = (byte)(byte.MaxValue * color.r);
        byte g = (byte)(byte.MaxValue * color.g);
        byte b = (byte)(byte.MaxValue * color.b);
        return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }

    void Update()
    {
        string colorString = GetHexString(InProgressTextColor);
        string inProgressPart = "<color=" + colorString + ">" + currentMessage + "</color>";
        outputText.text = messageLog.ToString() + inProgressPart;
    }

    private void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            currentMessage = "";
            messageLog.AppendLine(e.Result.Text);
        }
    }

    private void Recognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            currentMessage = e.Result.Text;
        }
    }

    private void Recognizer_SpeechStartDetected(object sender, RecognitionEventArgs e)
    {
    }

    private void OnDestroy()
    {
        if(recognizer != null)
        {
            recognizer.Dispose();
        }
    }
}
