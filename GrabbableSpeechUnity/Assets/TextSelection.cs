using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class TextSelection : MonoBehaviour
{
    public Transform SelectionStart;
    public Transform SelectionEnd;

    private SelectionCarat SelectionStartIndex;
    private SelectionCarat SelectionEndIndex;

    private StringBuilder testTextBuilder = new StringBuilder();
    private RectTransform canvasRect;

    private ISpeechSource speechSource;

    private void Start()
    {
        canvasRect = MainScript.Instance.Canvas.GetComponent<RectTransform>();
        speechSource = new FakeSpeechGenerator(); // Replace with SpeechRecognitionManager later
    }

    private void Update()
    {
        Vector2 visibleCharactersCount = MainScript.Instance.VisibleCharactersCount;
        UpdateSelection(visibleCharactersCount);
        MainScript.Instance.OutputTextObj.text = GetLogText(speechSource.Blocks, visibleCharactersCount);
    }

    private void UpdateSelection(Vector2 visibleCharactersCount)
    {
        Plane plane = new Plane(canvasRect.forward, canvasRect.position);
        SelectionStartIndex = GetCarat(SelectionStart.position, plane, visibleCharactersCount);
        SelectionEndIndex = GetCarat(SelectionEnd.position, plane, visibleCharactersCount);
    }

    private SelectionCarat GetCarat(Vector3 caratPos, Plane canvasPlane, Vector2 visibleCharactersCount)
    {
        Vector3 projectedStart = canvasPlane.ClosestPointOnPlane(caratPos);
        Vector3 transformedStart = canvasRect.InverseTransformPoint(projectedStart);

        float xParam = Mathf.Clamp01(transformedStart.x / canvasRect.rect.width + .5f);
        float xIndex = (visibleCharactersCount.x * xParam);

        float yParam = Mathf.Clamp01(transformedStart.y / canvasRect.rect.height + .5f);
        yParam = 1 - yParam;
        float yIndex = (visibleCharactersCount.y * yParam);
        return new SelectionCarat(yParam, xParam);
    }

    private string GetLogText(IEnumerable<SpeechBlock> speechBlocks, Vector2 visibleCharactersCount)
    {
        testTextBuilder.Clear();
        int lineIndex = 0;
        int relativeStartLine = (int)(SelectionStartIndex.VerticalLineIndex * visibleCharactersCount.y);
        int relativeEndLine = (int)(SelectionEndIndex.VerticalLineIndex * visibleCharactersCount.y);
        int totalLines = speechBlocks.Sum(item => item.Lines.Count);
        int startLine = totalLines - (int)visibleCharactersCount.y + relativeStartLine;
        int endLine = totalLines - (int)visibleCharactersCount.y + relativeEndLine;
        foreach (SpeechBlock block in speechBlocks)
        {
            foreach (string line in block.Lines)
            {
                string toAppend = line;
                if (lineIndex == startLine)
                {
                    int characterIndex = (int)(SelectionStartIndex.HorizontalCharacterIndex * visibleCharactersCount.x);
                    toAppend = GetModifiedLine(toAppend, "<color=#00FFFF>", characterIndex);
                }
                if (lineIndex == endLine)
                {
                    int characterIndex = (int)(SelectionEndIndex.HorizontalCharacterIndex * visibleCharactersCount.x);
                    toAppend = GetModifiedLine(toAppend, "</color>", characterIndex);
                }
                testTextBuilder.AppendLine(toAppend);
                lineIndex++;
            }
        }
        return testTextBuilder.ToString();
    }

    private string GetModifiedLine(string toModify, string toInsert, int horizontalCharacterIndex)
    {
        int clippedIndex = Mathf.Min(horizontalCharacterIndex, toModify.Length);
        string pre = toModify.Substring(0, clippedIndex);
        string post = toModify.Substring(clippedIndex, toModify.Length - clippedIndex);
        return pre + toInsert + post;
    }

    private struct SelectionCarat
    {
        public float VerticalLineIndex { get; }
        public float HorizontalCharacterIndex{ get; }

        public SelectionCarat(float verticalLineIndex, float horizontalCharacterIndex)
        {
            VerticalLineIndex = verticalLineIndex;
            HorizontalCharacterIndex = horizontalCharacterIndex;
        }
    }
}
