using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class TextSelection : MonoBehaviour
{
    public static TextSelection Instance;

    private SelectionCarat SelectionStartIndex;
    private SelectionCarat SelectionEndIndex;

    private StringBuilder testTextBuilder = new StringBuilder();
    private RectTransform canvasRect;
    private RectTransform logTextRect;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvasRect = MainScript.Instance.Canvas.GetComponent<RectTransform>();
        logTextRect = MainScript.Instance.LogTextObj.GetComponent<RectTransform>();
    }

    public string GetLogText()
    {
        UpdateSelection(MainScript.Instance.VisibleCharactersCount);
        return GetLogText(MainScript.Instance.SpeechSource.Blocks, MainScript.Instance.VisibleCharactersCount);
    }

    private void UpdateSelection(Vector2 visibleCharactersCount)
    {
        Plane plane = new Plane(canvasRect.forward, canvasRect.position);

        SelectionCarat caratA = GetCarat(MainScript.Instance.SelectionStart.position, plane, visibleCharactersCount);
        SelectionCarat caratB = GetCarat(MainScript.Instance.SelectionEnd.position, plane, visibleCharactersCount);
        
        int caratALine = (int)(caratA.VerticalLineIndex * visibleCharactersCount.y);
        int caratBLine = (int)(caratB.VerticalLineIndex * visibleCharactersCount.y);
        if(caratALine == caratBLine)
        {
            if(caratA.HorizontalCharacterIndex < caratB.HorizontalCharacterIndex)
            {
                SelectionStartIndex = caratA;
                SelectionEndIndex = caratB;
            }
            else
            {
                SelectionStartIndex = caratB;
                SelectionEndIndex = caratA;
            }
        }
        else if(caratALine > caratBLine)
        {
            SelectionStartIndex = caratA;
            SelectionEndIndex = caratB;
        }
        else
        {
            SelectionStartIndex = caratB;
            SelectionEndIndex = caratA;
        }
    }

    private SelectionCarat GetCarat(Vector3 caratPos, Plane canvasPlane, Vector2 visibleCharactersCount)
    {
        Vector3 projectedStart = canvasPlane.ClosestPointOnPlane(caratPos);
        Vector3 transformedStart = canvasRect.InverseTransformPoint(projectedStart);

        float xParam = Mathf.Clamp01(transformedStart.x / canvasRect.rect.width + .5f);
        float yParam = Mathf.Clamp01(transformedStart.y / canvasRect.rect.height + .5f);
        yParam -= logTextRect.offsetMin.y / canvasRect.rect.width;
        return new SelectionCarat(yParam, xParam);
    }
   

    private string GetLogText(IEnumerable<SpeechBlock> speechBlocks, Vector2 visibleCharactersCount)
    {
        int totalLines = speechBlocks.Sum(item => item.Lines.Count);
        int totalVisibleLines = (int)visibleCharactersCount.y;
        int startLine = (int)(SelectionStartIndex.VerticalLineIndex * visibleCharactersCount.y);
        startLine = Mathf.Min(totalLines - 1, startLine);

        int endLine = (int)(SelectionEndIndex.VerticalLineIndex * visibleCharactersCount.y);
        endLine = Mathf.Min(totalLines - 1, endLine);

        return GetLogText(speechBlocks, totalLines, startLine, endLine, visibleCharactersCount.x);
    }

    private string GetLogText(IEnumerable<SpeechBlock> speechBlocks, int totalLines, int startLine, int endLine, float charactersPerLine)
    {
        int lineIndex = totalLines;
        testTextBuilder.Clear();
        foreach (SpeechBlock block in speechBlocks)
        {
            foreach (string line in block.Lines)
            {
                lineIndex--;
                string toAppend = line;
                if (lineIndex == endLine)
                {
                    int characterIndex = (int)(SelectionEndIndex.HorizontalCharacterIndex * charactersPerLine);
                    toAppend = GetModifiedLine(toAppend, "</color>", characterIndex);
                }
                if (lineIndex == startLine)
                {
                    int characterIndex = (int)(SelectionStartIndex.HorizontalCharacterIndex * charactersPerLine);
                    characterIndex = Mathf.Min(line.Length, characterIndex);
                    toAppend = GetModifiedLine(toAppend, "<color=#00FFFF>", characterIndex);
                }
                testTextBuilder.AppendLine(toAppend);
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
