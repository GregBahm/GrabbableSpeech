using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TextSelection : MonoBehaviour
{
    public Transform SelectionStart;
    public Transform SelectionEnd;

    public int SelectionStartIndex;
    public int SelectionEndIndex;

    private StringBuilder testTextBuilder = new StringBuilder();
    private RectTransform canvasRect;
    
    private void Start()
    {
        canvasRect = MainScript.Instance.Canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        UpdateSelection();
        MainScript.Instance.OutputTextObj.text = GetTestText();
    }

    public float DebugX;
    public float DebugY;

    private void UpdateSelection()
    {
        Vector2 visibleCharactersCount = GetVisibleCharactersCount();
        Plane plane = new Plane(canvasRect.forward, canvasRect.position);
        SelectionStartIndex = GetCharacterIndex(SelectionStart.position, plane, visibleCharactersCount);
        SelectionEndIndex = GetCharacterIndex(SelectionEnd.position, plane, visibleCharactersCount);
    }

    private int GetCharacterIndex(Vector3 caratPos, Plane canvasPlane, Vector2 visibleCharactersCount)
    {
        Vector3 projectedStart = canvasPlane.ClosestPointOnPlane(caratPos);
        Vector3 transformedStart = canvasRect.InverseTransformPoint(projectedStart);

        float xParam = Mathf.Clamp01(transformedStart.x / canvasRect.rect.width + .5f);
        int xIndex = (int)(visibleCharactersCount.x * xParam);

        float yParam = Mathf.Clamp01(transformedStart.y / canvasRect.rect.height + .5f);
        yParam = 1 - yParam;
        int yIndex = (int)(visibleCharactersCount.y * yParam);

        DebugX = xParam;
        DebugY = yParam;

        yIndex *= Mathf.FloorToInt(visibleCharactersCount.x);
        return yIndex + Mathf.FloorToInt(xIndex);
    }

    public Vector2 GetVisibleCharactersCount()
    {
        float vertical = canvasRect.rect.width * MainScript.Instance.OutputTextObj.fontSize * 40000;
        float horzontal = canvasRect.rect.width * MainScript.Instance.OutputTextObj.fontSize * (13f / 7) * 10000;
        return new Vector2(vertical, horzontal);
    }

    private string GetTestText()
    {
        testTextBuilder.Clear();
        int characterIndex = 0;
        for (int y = 0; y < 13; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                if(characterIndex == SelectionStartIndex)
                {
                    testTextBuilder.Append("<color=#00FFFF>");
                }
                if(characterIndex == SelectionEndIndex)
                {
                    testTextBuilder.Append("</color>");
                }
                testTextBuilder.Append(x % 9);
                characterIndex++;
            }
            testTextBuilder.Append("\n");
        }
        return testTextBuilder.ToString();
    }
}
