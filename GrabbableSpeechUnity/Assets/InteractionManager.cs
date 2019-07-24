using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float ScrollSensitivity;

    private PinchDetector leftPinchDetector;
    private PinchDetector rightPinchDetector;

    private float scrollPanelStartPos;
    private float scrollPinchStartPos;
    private bool wasScrollPinching;
    private float scrollTarget;
    private bool wasSelectPinching;
    
    private void Start()
    {
        JointTrackingManager tracker = JointTrackingManager.Instance;
        leftPinchDetector = new PinchDetector(tracker.LeftIndexProxy.gameObject, tracker.LeftThumbProxy.gameObject);
        rightPinchDetector = new PinchDetector(tracker.RightIndexProxy.gameObject, tracker.RightThumbProxy.gameObject);
    }

    private void Update()
    {
        leftPinchDetector.Update();
        rightPinchDetector.Update();

        HandleSelecting();
        HandleScrolling(); 
        MainScript.Instance.Scrolling = Mathf.Lerp(MainScript.Instance.Scrolling, scrollTarget, Time.deltaTime * 5);
    } 

    private void HandleScrolling()
    {
        if (leftPinchDetector.Pinching)
        {
            if (!wasScrollPinching)
            {
                scrollPanelStartPos = scrollTarget;
                scrollPinchStartPos = leftPinchDetector.PinchPos.y;
            }
            float scrollChange = scrollPinchStartPos - leftPinchDetector.PinchPos.y;
            scrollChange *= ScrollSensitivity;
            scrollTarget = scrollPanelStartPos - scrollChange;
        }
        else
        {
            scrollTarget = Mathf.Max(0, scrollTarget);
        }
        wasScrollPinching = leftPinchDetector.Pinching;
    }

    private void HandleSelecting()
    { 
        if (rightPinchDetector.Pinching) 
        {
            if (!wasSelectPinching)
            {
                MainScript.Instance.SelectionEnd.position = rightPinchDetector.PinchPos;
            }
            MainScript.Instance.SelectionStart.position = rightPinchDetector.PinchPos;
        }
        wasSelectPinching = rightPinchDetector.Pinching;  
    }
}
