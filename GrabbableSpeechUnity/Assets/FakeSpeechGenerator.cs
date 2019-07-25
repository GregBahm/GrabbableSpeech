using System;
using System.Collections.Generic;
using UnityEngine;

public class FakeSpeechGenerator : MonoBehaviour, ISpeechSource
{
    public int StartingBlockCount;
    public int MaxSentanceLength;
    public int MaxWordLength;
    public float UpdateFrequency;
    private float nextUpdate;

    public static FakeSpeechGenerator Instance;

    private int index;

    private List<SpeechBlock> blocks;
    public IEnumerable<SpeechBlock> Blocks { get { return blocks; } }
    public string SpeechInProgress { get { return "in progress speech"; } }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        blocks = GenerateBlocks();
        nextUpdate = UpdateFrequency;
    }

    void Update()
    {
        nextUpdate -= Time.deltaTime;
        if(nextUpdate < 0)
        {
            nextUpdate = UpdateFrequency;
            SpeechBlock block = GetRandomNonsenseSpeechblock();
            blocks.Add(block);
        }
    }

    private List<SpeechBlock> GenerateBlocks()
    {
        List<SpeechBlock> ret = new List<SpeechBlock>();
        for (int blocksCount = 0; blocksCount < StartingBlockCount; blocksCount++)
        {
            SpeechBlock block = GetRandomNonsenseSpeechblock();
            ret.Add(block);
        }
        return ret;
    }

    private SpeechBlock GetRandomNonsenseSpeechblock()
    {
        index++;
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
