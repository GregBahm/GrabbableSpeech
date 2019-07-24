using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointTrackingManager : MonoBehaviour
{
    public static JointTrackingManager Instance;

    public Transform LeftIndexProxy;
    public Transform LeftThumbProxy;

    public Transform RightIndexProxy;
    public Transform RightThumbProxy;

    private void Start()
    {
        Instance = this;
    }
    
    private void Update()
    {
        UpdateProxy(LeftIndexProxy, Handedness.Left, TrackedHandJoint.IndexTip);
        UpdateProxy(LeftThumbProxy, Handedness.Left, TrackedHandJoint.ThumbTip);
        UpdateProxy(RightIndexProxy, Handedness.Right, TrackedHandJoint.IndexTip);
        UpdateProxy(RightThumbProxy, Handedness.Right, TrackedHandJoint.ThumbTip);
    }

    private void UpdateProxy(Transform target, Handedness hand, TrackedHandJoint jont)
    {
        if (HandJointUtils.TryGetJointPose(jont, hand, out MixedRealityPose pose))
        {
            target.position = pose.Position;
            target.rotation = pose.Rotation;
        }
    }
}
