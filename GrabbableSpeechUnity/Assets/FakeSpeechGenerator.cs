using System;
using System.Collections.Generic;
using UnityEngine;

public class FakeSpeechGenerator : ISpeechSource
{
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
        for (int blocksCount = 0; blocksCount < 20; blocksCount++)
        {
            SpeechBlock block = GetRandomNonsenseSpeechblock(blocksCount);
            ret.Add(block);
        }
        return ret;
    }

    private SpeechBlock GetRandomNonsenseSpeechblock(int index)
    {
        int wordsCount = (int)(UnityEngine.Random.value * 10);
        string sentence = "";
        for (int wordIndex = 0; wordIndex < wordsCount; wordIndex++)
        {
            int lettersCount = (int)(UnityEngine.Random.value * 5) + 1;
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
