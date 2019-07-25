using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public float PinchDist;
    public static PanelManager Instance;

    public Color GlowColor;
    public PanelAndSummoner[] Panels;
    public float HighlightingDuration;

    public float HandSummonerProximityThreshold;
    public float SummonSolidifyTime;
    private float solidificationRemaining;

    public float SummonSolidification { get; private set; }
    public bool ShowSummoner { get; private set; }
    private bool wasShowingSummoner;

    public Transform SummonerReadyPosition;
    public SummonerInteraction HoveredSummoner { get; private set; }
    private SummonerInteraction SelectedSummoner;
    private bool distancePinch;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        bool isHandCloseToSummoner = GetIsHandCloseToSummoner();
        PlaceSummnerReadyPosition(isHandCloseToSummoner);
        SetHoveredSummoner();
        SetSelectedSummoner();
        SetShaderGlobals();

        ShowSummoner = GetShouldShowSummoner(isHandCloseToSummoner);
        if(ShowSummoner && !wasShowingSummoner)
        {
            solidificationRemaining = SummonSolidifyTime;
        }
        UpdateSolidification();
        wasShowingSummoner = ShowSummoner;
    }
    private void PlaceSummnerReadyPosition(bool isHandCloseToSummoner)
    {
        Vector3 targetPos = JointTrackingManager.Instance.PalmProxy.position + new Vector3(0, 0.14f, 0);
        if (ShowSummoner)
        {
            SummonerReadyPosition.position = Vector3.Lerp(SummonerReadyPosition.position, targetPos, PanelManager.Instance.SummonSolidification);
        }
        else
        {
            SummonerReadyPosition.position = targetPos;
        }
        
        SummonerReadyPosition.LookAt(Camera.main.transform, Vector3.up);
    }

    private bool GetShouldShowSummoner(bool isHandCloseToSummoner)
    {
        if (GetIsPalmFacing())
        {
            return true;
        }
        return ShowSummoner && isHandCloseToSummoner;
    }

    private bool GetIsHandCloseToSummoner()
    {
        Vector3 handPos = JointTrackingManager.Instance.PalmProxy.position;
        Vector3 summonerPos = Panels[0].Summoner.transform.position;
        float dist = (handPos - summonerPos).magnitude;
        return dist < HandSummonerProximityThreshold;
    }

    private void UpdateSolidification()
    {
        SummonSolidification = Mathf.Clamp01(solidificationRemaining);
        solidificationRemaining -= Time.deltaTime;
    }

    public bool GetIsPalmFacing()
    {
        float facingDot = Vector3.Dot(JointTrackingManager.Instance.PalmProxy.up, Camera.main.transform.forward);
        return facingDot > .65f;
    }

    private void SetHoveredSummoner()
    {
        float closestDist = PinchDist;
        HoveredSummoner = null;
        foreach (SummonerInteraction item in Panels.Select(item => item.Summoner))
        {
            float pinchDist = (PinchDetector.Instance.PinchPos - item.SliderCore.position).magnitude;
            if (pinchDist < closestDist)
            {
                closestDist = pinchDist;
                HoveredSummoner = item;
            }
        }
    }


    private void SetSelectedSummoner()
    {
        if (PinchDetector.Instance.Pinching && !distancePinch && SelectedSummoner == null)
        {
            if(Panels.All(item => item.Summoner.State == SummonerState.Ready))
            {
                float closestDist = PinchDist;
                SelectedSummoner = null;
                foreach (SummonerInteraction item in Panels.Select(item => item.Summoner))
                {

                    float pinchDist = (PinchDetector.Instance.PinchPos - item.SliderCore.position).magnitude;
                    if (pinchDist < closestDist)
                    {
                        closestDist = pinchDist;
                        SelectedSummoner = item;
                    }
                }
            }
            if(SelectedSummoner == null)
            {
                distancePinch = true;
            }
            else
            {
                SelectedSummoner.StartPinching(PinchDetector.Instance.PinchPos);
            }
        }
        else
        {
            SelectedSummoner = null;
            distancePinch = false;
        }
    }

    private void SetShaderGlobals()
    {
        Shader.SetGlobalVector("_GrabPoint", JointTrackingManager.Instance.IndexProxy.position);
        float grabbedness = Panels.Max(item => item.Plate.Grabbedness);
        Shader.SetGlobalFloat("_GlobalGrabbedness", grabbedness);
        Shader.SetGlobalColor("_GrabbedColor", GlowColor);
    }
}

[Serializable]
public class PanelAndSummoner
{
    public BackplateScript Plate;
    public SummonerInteraction Summoner;
}
