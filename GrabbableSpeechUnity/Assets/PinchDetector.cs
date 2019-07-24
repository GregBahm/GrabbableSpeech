using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchDetector
{
    private readonly GameObject IndexProxy;
    private readonly GameObject ThumbProxy;

    public bool Pinching { get; private set; }

    public Vector3 PinchPos { get; private set; }

    public PinchDetector(GameObject indexProxy, GameObject thumbProxy)
    {
        IndexProxy = indexProxy;
        ThumbProxy = thumbProxy;
    }
    
    public void Update()
    {
        Pinching = false;

        float tipDistance = (IndexProxy.transform.position - ThumbProxy.transform.position).magnitude;
        Pinching = tipDistance < ThumbProxy.transform.localScale.x;
        
        PinchPos = (IndexProxy.transform.position + ThumbProxy.transform.position) / 2;
    }
}
