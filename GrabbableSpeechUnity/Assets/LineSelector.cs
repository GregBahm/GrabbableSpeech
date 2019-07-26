using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LineSelector : MonoBehaviour
{
    private enum SelectorState
    {
        Inactive,
        Hovering,
        Pinching,
    }

    public float HoverDistance;
    public static LineSelector Instance;
    
    public GameObject LinePrefab; 
    public Transform HoverVisual;
    public BoxCollider Hoverbox;

    private SelectorState state;

    private RectTransform canvasRect;
    private RectTransform logTextRect;

    private SelectorState lastState;
    private StringBuilder textBuilder = new StringBuilder();

    private GameObject currentAnnotation;
    private List<SpeechBlock> speechBlocks = new List<SpeechBlock>();

    private string annotationLine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvasRect = MainScript.Instance.Canvas.GetComponent<RectTransform>();
        logTextRect = MainScript.Instance.LogTextObj.GetComponent<RectTransform>();
    } 

    private void Update()
    {
        bool isPinching = PinchDetector.Instance.Pinching; 
        
        if (isPinching)
        {
            if (state == SelectorState.Hovering && isPinching)
            {
                StartPinch();
            }
            state = SelectorState.Pinching;
        }
        else
        {
            if(state == SelectorState.Pinching && currentAnnotation != null)
            {
                currentAnnotation.transform.parent = null;
            }
            bool shouldHover = GetShouldHover();
            if (shouldHover) 
            {
                state = SelectorState.Hovering;
                UpdateHovering();
            }
            else
            {
                state = SelectorState.Inactive; 
            }
        }
        //HoverVisual.gameObject.SetActive(state == SelectorState.Hovering);


        if (MainScript.Instance.AllowLogChanges && state != SelectorState.Hovering)
        {
            speechBlocks = new List<SpeechBlock>(MainScript.Instance.SpeechSource.Blocks);
        }

    } 

    private bool GetShouldHover()
    {
        Vector3 fingerTip = JointTrackingManager.Instance.IndexProxy.position;
        Vector3 hoverPoint = Hoverbox.ClosestPoint(fingerTip);
        float dist = (fingerTip - hoverPoint).magnitude;
        return dist < HoverDistance;
    }

    private int lineToHighlightIndex;

    private void UpdateHovering()
    {
        Vector3 caratPosition = JointTrackingManager.Instance.IndexProxy.position;
        float yParam = GetVerticalLineParam(caratPosition);
        lineToHighlightIndex = (int)(yParam * MainScript.Instance.VisibleCharactersCount.y);

        HoverVisual.localPosition = GetSelectorTabPos(yParam);
    }

    private Vector3 GetSelectorTabPos(float yParam)
    {
        float top = canvasRect.rect.height / 2;
        float bottom = -canvasRect.rect.height / 2;
        float y = Mathf.Lerp(bottom, top, yParam);
        return new Vector3(HoverVisual.localPosition.x, y, HoverVisual.localPosition.z);
    }

    private void StartPinch()
    {
        GameObject newAnnotation = Instantiate(LinePrefab);
        newAnnotation.transform.parent = JointTrackingManager.Instance.PalmProxy;
        newAnnotation.transform.position = HoverVisual.position;
        newAnnotation.transform.rotation = HoverVisual.rotation;
        currentAnnotation = newAnnotation;

        AnnotationItem annotation = newAnnotation.GetComponent<AnnotationItem>();
        annotation.TextMesh.text = annotationLine;
    }

    public string GetLogText()
    {
        int lineIndex = speechBlocks.Sum(item => item.Lines.Count); 
        textBuilder.Clear();
        foreach (SpeechBlock block in speechBlocks)
        {
            foreach (string line in block.Lines)
            {
                lineIndex--;
                if (state != SelectorState.Inactive && lineIndex == lineToHighlightIndex)
                {
                    string newline = "<mark=#0099FF55>" + line + "</mark>";
                    textBuilder.AppendLine(newline);
                    annotationLine = line;
                }
                else
                {
                    textBuilder.AppendLine(line);
                }
            }
        }
        return textBuilder.ToString();
    }

    private float GetVerticalLineParam(Vector3 fingerPos)
    {
        Plane canvasPlane = new Plane(canvasRect.forward, canvasRect.position);
        Vector3 projectedStart = canvasPlane.ClosestPointOnPlane(fingerPos);
        Vector3 transformedStart = canvasRect.InverseTransformPoint(projectedStart);
        
        float yParam = Mathf.Clamp01(transformedStart.y / canvasRect.rect.height + .5f);
        yParam -= logTextRect.offsetMin.y / canvasRect.rect.width;
        return yParam;
    }
}
