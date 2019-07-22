using System;
using System.Collections.Generic;

public class SpeechBlock
{
    public DateTime TimeStamp { get; }
    public string BaseText { get; }
    public IReadOnlyList<string> Lines { get; }
    
    public SpeechBlock(string text, DateTime timeStamp, int charactersPerLine)
    {
        BaseText = text;
        TimeStamp = timeStamp;
        Lines = GetLines(text, charactersPerLine);
    }

    private List<string> GetLines(string text, int charactersPerLine)
    {
        List<string> ret = new List<string>();
        string[] words = text.Split(' ');
        int lineLength = 0;
        string currentLine = "";
        foreach (string word in words)
        {
            lineLength += word.Length;
            if(lineLength > charactersPerLine)
            {
                ret.Add(currentLine);
                currentLine = word + " ";
                lineLength = word.Length + 1;
            }
            else
            {
                currentLine += word + " ";
                lineLength += 1;
            }
        }
        ret.Add(currentLine);
        return ret;
    }
}