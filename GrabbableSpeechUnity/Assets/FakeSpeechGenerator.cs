using System;
using System.Collections.Generic;
using UnityEngine;

public class FakeSpeechGenerator : ISpeechSource
{
    public const int BlockCount = 50 ;
    public const int MaxSentanceLength = 10;
    public const int MaxWordLength = 5;

    private readonly List<SpeechBlock> blocks;
    public IEnumerable<SpeechBlock> Blocks { get { return blocks; } }
    public string SpeechInProgress { get { return "in progress speech"; } }

    public FakeSpeechGenerator()
    {
        blocks = GenerateBlocks();
    }

    private List<SpeechBlock> GenerateBlocks()
    {
        List<SpeechBlock> ret = new List<SpeechBlock>();
        for (int blocksCount = 0; blocksCount < BlockCount; blocksCount++)
        {
            SpeechBlock block = GetRandomNonsenseSpeechblock(blocksCount);
            ret.Add(block);
        }
        return ret;
    }

    private SpeechBlock GetRandomNonsenseSpeechblock(int index)
    {
        int wordsCount = (int)(UnityEngine.Random.value * MaxSentanceLength);
        string sentence = "";
        for (int wordIndex = 0; wordIndex < wordsCount; wordIndex++)
        {
            int lettersCount = (int)(UnityEngine.Random.value * MaxWordLength) + 1;
            for (int letterCount = 0; letterCount < lettersCount; letterCount++)
            {
                sentence += index.ToString();
            }
            sentence += " ";
        }
        int charactersPerLine = Mathf.FloorToInt(MainScript.Instance.VisibleCharactersCount.x);
        return new SpeechBlock(sentence.Trim(), DateTime.Now, charactersPerLine);
    }
}
