using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float ScrollSensitivity;

    private PinchDetector leftPinchDetector;
    private PinchDetector rightPinchDetector;
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
