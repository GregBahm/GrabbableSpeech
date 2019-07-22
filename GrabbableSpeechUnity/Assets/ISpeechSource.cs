using System.Collections.Generic;

public interface ISpeechSource
{
    IEnumerable<SpeechBlock> Blocks { get; }
    string SpeechInProgress { get; }
}
