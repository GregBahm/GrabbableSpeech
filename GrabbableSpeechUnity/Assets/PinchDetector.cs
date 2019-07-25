using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchDetector : MonoBehaviour
{
    public static PinchDetector Instance;

    public GameObject IndexProxy;
    public GameObject ThumbProxy;

    public bool Pinching { get; private set; }

    public Vector3 PinchPos { get; private set; }

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        Pinching = false;

        float tipDistance = (IndexProxy.transform.position - ThumbProxy.transform.position).magnitude;
        Pinching = tipDistance < ThumbProxy.transform.localScale.x;

        PinchPos = (IndexProxy.transform.position + ThumbProxy.transform.position) / 2;
    }
}
