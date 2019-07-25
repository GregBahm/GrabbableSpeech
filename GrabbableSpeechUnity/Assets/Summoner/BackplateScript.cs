using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackplateScript : MonoBehaviour
{
    private Vector3 resetScale;
    public Transform PositionTarget { get; private set; }

    public bool SlateIsGrabbed { get; private set; }
    private bool wasDragging;
    public float Grabbedness { get; private set; }

    void Start()
    {
        PositionTarget = new GameObject(gameObject.name + " positionTarget").transform;
        PositionTarget.position = transform.position;
        resetScale = transform.localScale;
    }

    void Update()
    {
        if (SlateIsGrabbed)
        {
            PositionTarget.position = transform.position;
            PositionTarget.rotation = transform.rotation;
        }
        else
        {
            UpdateSlatePosition();
        }
        CorrectRotation();
    }

    private void CorrectRotation()
    {
        Vector3 euler = transform.eulerAngles;
        Quaternion quant = Quaternion.Euler(euler.x, euler.y, 0);
        transform.rotation = quant;
    }

    private void UpdateSlatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, PositionTarget.position, Time.deltaTime * 7);
        transform.rotation = Quaternion.Lerp(transform.rotation, PositionTarget.rotation, Time.deltaTime * 7);
    }

    public void OnSlateGrab()
    {
        SlateIsGrabbed = true;
    }

    public void OnSlateRelease()
    {
        SlateIsGrabbed = false;
    }
}
