using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerInteraction : MonoBehaviour
{
    [Range(0, 1)]
    public float SlideProgress;
    public SummonerState State;
    public Transform ReadyPositionTarget;
    public Transform UnlockPoint;
    public Transform SlateSummonPoint;
    public Transform SliderCore;
    public BackplateScript SlatePositioner;
    
    private Vector3 pinchStartPoint;

    private Vector3 summonerTargetPos;
    private Quaternion summonerTargetRot;

    private void Start() 
    {
    }

    private void Update()
    {
        UpdateShowHide();
        if(State != SummonerState.Hidden)
        {
            UpdateState();
        }
    }

    private void UpdateShowHide()
    {
        if (State == SummonerState.Hidden || State == SummonerState.Ready)
        {
            State = PanelManager.Instance.ShowSummoner ? SummonerState.Ready : SummonerState.Hidden;
        }
    }

    private void UpdateState()
    {
        if(PinchDetector.Instance.Pinching)
        {
            if (State == SummonerState.Pinched)
            {
                UpdatePinchingState();
            }
            if(State == SummonerState.Placing)
            {
                UpdatePlacingState();
            }
        }
        else
        {
            State = SummonerState.Ready;
            UpdateReadyState();
        }
    }

    public void StartPinching(Vector3 pinchStartPos)
    {
        State = SummonerState.Pinched;
        pinchStartPoint = pinchStartPos;
    }

    private void UpdatePinchingState()
    {
        Vector3 dragVector = PinchDetector.Instance.PinchPos - pinchStartPoint;
        Vector3 projected = Vector3.Project(dragVector, transform.right);
        Vector3 draggedPoint = transform.position + projected;

        float distToUnlock = (UnlockPoint.position - draggedPoint).magnitude;
        float distFromStart = (transform.position - draggedPoint).magnitude;

        float unlockDist = (UnlockPoint.position - transform.position).magnitude;
        float paramFromStart = distFromStart / unlockDist;
        float paramFromEnd = distToUnlock / unlockDist;

        SlideProgress = 1 - Mathf.Clamp01(paramFromEnd);
        if (paramFromStart > 1 && paramFromStart > paramFromEnd)
        {
            SlideProgress = 1;
        }
        if (SlideProgress > .99f)
        {
            State = SummonerState.Placing;
        }
    }

    private void UpdatePlacingState()
    {
        summonerTargetPos = PinchDetector.Instance.PinchPos;
        SlatePositioner.PositionTarget.position = SlateSummonPoint.position;
        SlatePositioner.PositionTarget.rotation = SlateSummonPoint.rotation;

        transform.LookAt(Camera.main.transform, Vector3.up);
        transform.Rotate(0, 180, -90);

        transform.position = Vector3.Lerp(transform.position, summonerTargetPos, Time.deltaTime * 8);
    }

    private void UpdateReadyState()
    {
        SlideProgress = 0;
        summonerTargetPos = ReadyPositionTarget.position;
        transform.position = summonerTargetPos;
        transform.LookAt(Camera.main.transform, Vector3.up);
        transform.Rotate(0, 180, -90);
    }
}
