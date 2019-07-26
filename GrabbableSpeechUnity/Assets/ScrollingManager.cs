using System;
using System.Linq;
using UnityEngine;

public class ScrollingManager : MonoBehaviour
{
    public static ScrollingManager Instance;
    public Transform Scrollbar;

    public BoxCollider SwipeZone;

    public float Scrollage { get; set; }
    
    private float scrollPanelStartPos;
    private float scrollPinchStartPos;
    public float scrollTarget;
    private bool wasScrollPinching;
    private RectTransform canvasRect;
    private bool scrollbarIsVisible;
    private int linesCountLastFrame;

    void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        canvasRect = MainScript.Instance.Canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        int linesCount = MainScript.Instance.SpeechSource.Blocks.Sum(block => block.Lines.Count);
        float pagesCount = GetPagesCount(linesCount); 
        float scrollMax = GetScrollMax(pagesCount);
        scrollbarIsVisible = scrollMax > 0;
        scrollMax = Mathf.Max(0, scrollMax);

        MainScript.Instance.AllowLogChanges = Mathf.Abs(scrollTarget) < float.Epsilon;
        HandleScrollClamping(scrollMax);

        Scrollage = Mathf.Lerp(Scrollage, scrollTarget, Time.deltaTime * 5);
        UpdateScrollbar(pagesCount, scrollMax);
        UpdateSwipeMode(); 
    }

    private void UpdateScrollbar(float pagesCount, float scrollMax)
    {
        Scrollbar.gameObject.SetActive(scrollbarIsVisible);
        if(!scrollbarIsVisible)
        {
            return;
        }

        float maxHeight = canvasRect.rect.height;
        float proportion = 1 / (pagesCount + .5f);
        float scrollbarHeight = (maxHeight * proportion) / 2;
        Vector3 scrollbarScale = new Vector3(Scrollbar.localScale.x, scrollbarHeight, Scrollbar.localScale.z);
        Scrollbar.localScale = scrollbarScale;

        float scrollParam = 1 - (Scrollage / -scrollMax);
        float scrollbarBottom = canvasRect.rect.height / 2 - scrollbarHeight / 2;
        float scrollbarTop = -canvasRect.rect.height / 2 + scrollbarHeight / 2;
        float scrollbarPos = Mathf.Lerp(scrollbarBottom, scrollbarTop, scrollParam);
        Scrollbar.localPosition = new Vector3(Scrollbar.localPosition.x, scrollbarPos, Scrollbar.localPosition.z);
    }

    private float GetPagesCount(int linesCount)
    {
        float visibleCharactersCount = MainScript.Instance.VisibleCharactersCount.y;
        float pagesCount = (linesCount / visibleCharactersCount) - 1f;
        return pagesCount;
    }

    private float GetScrollMax(float pagesCount)
    {
        float span = canvasRect.rect.height;
        float scrollMax = pagesCount * span;
        return scrollMax;
    }

    private void HandleScrollClamping(float scrollMax)
    {
        scrollTarget = Mathf.Clamp(scrollTarget, -scrollMax, 0);
    }


    private bool GetIsSwipeEntered()
    {
        Vector3 indexPosition = JointTrackingManager.Instance.IndexProxy.position;
        return SwipeZone.bounds.Contains(indexPosition);
    }

    private bool wasSwipeModeEntered;
    float swipeFingerStartPos;
    float swipeScrollStartPos;
    private float GetFingerRelativeToFingerzone()
    {
        Vector3 indexPosition = JointTrackingManager.Instance.IndexProxy.position;
        Vector3 relativePos = SwipeZone.transform.worldToLocalMatrix * indexPosition;
        return relativePos.y;
    }

    public float SwipeModeSensitivity;

    private void UpdateSwipeMode()
    {
        bool isEntered = GetIsSwipeEntered();
        if ((isEntered || wasSwipeModeEntered))
        {
            float relativeFinger = GetFingerRelativeToFingerzone();
            if (!wasSwipeModeEntered)
            {
                swipeFingerStartPos = relativeFinger;
                swipeScrollStartPos = scrollTarget;
            }
            float scrollChange = swipeFingerStartPos - relativeFinger;
            scrollChange *= SwipeModeSensitivity;
            scrollTarget = swipeScrollStartPos + scrollChange;
        }
        wasSwipeModeEntered = isEntered;
    }
}