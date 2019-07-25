using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointTrackingManager : MonoBehaviour
{
    public static JointTrackingManager Instance;

    public Transform PalmProxy;
    public Transform IndexProxy;
    public Transform ThumbProxy;

    public bool IndexFound;
    private Handedness handedness;
    private bool indexFoundLastFrame;

    private void Start()
    {
        Instance = this;
    }


    private void Update()
    {
        UpdateIndex();
        UpdatePalm();
        UpdateThumb();
        indexFoundLastFrame = IndexFound;
    }

    private void UpdateThumb()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, handedness, out MixedRealityPose thumb))
        {
            ThumbProxy.position = thumb.Position;
            ThumbProxy.rotation = thumb.Rotation;
        }
    }

    private void UpdatePalm()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, handedness, out MixedRealityPose palm))
        {
            PalmProxy.position = palm.Position;
            PalmProxy.rotation = palm.Rotation;
        }
    }

    private void UpdateIndex()
    {
#if UNITY_EDITOR
        IndexFound = true;
#endif
        IndexFound = false;
        if (indexFoundLastFrame)
        {
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, handedness, out MixedRealityPose lastTip))
            {
                IndexProxy.position = lastTip.Position;
                IndexFound = true;
            }
            return;
        }
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose rightTip))
        {
            IndexProxy.position = rightTip.Position;
            handedness = Handedness.Right;
            IndexFound = true;
        }
        else if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out MixedRealityPose leftTip))
        {
            IndexProxy.position = leftTip.Position;
            handedness = Handedness.Left;
            IndexFound = true;
        }
    }
}
