using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SummonerInteraction))]
public class SummonerVisuals : MonoBehaviour
{
    private SummonerInteraction interaction;

    private float highlightingDurationRemaining;

    public Transform ElementsRoot;
    public Transform InnerMeshStartJoint;
    public Transform InnerMeshEndJoint;
    public Transform OuterMeshStartJoint;
    public Transform OuterMeshEndJoint;

    public Transform Thumbnail;

    private Transform summonStartPoint;
    private Transform summonEndPoint;

    private Material innerMeshMat;
    private Material outerMeshMat;
    private Material highlightMat;
    public SkinnedMeshRenderer InnerMeshRenderer;
    public SkinnedMeshRenderer OuterMeshRenderer;
    public MeshRenderer HighlightQuadRenderer;

    private float activeness;
    private float showness;
    private float thumbShowness;
    private float hoverness;

    private void Start()
    {
        interaction = GetComponent<SummonerInteraction>();
        innerMeshMat = InnerMeshRenderer.material;
        outerMeshMat = OuterMeshRenderer.material;
        highlightMat = HighlightQuadRenderer.material;

        summonStartPoint = new GameObject("Summon Start Point").transform;
        summonStartPoint.parent = InnerMeshStartJoint.parent.transform;
        summonStartPoint.position = InnerMeshStartJoint.position;

        summonEndPoint = new GameObject("Summon End Point").transform;
        summonEndPoint.parent = OuterMeshEndJoint.parent.transform;
        summonEndPoint.position = OuterMeshEndJoint.position;
    }

    private void Update()
    {
        UpdateShowHide();
        UpdateHover();
        UpdateHighlighting();
        float endingProg = highlightingDurationRemaining / PanelManager.Instance.HighlightingDuration;
        float endProgA = Mathf.Clamp01(endingProg * 2);
        float endProgB = Mathf.Clamp01(endingProg - .5f) * 2;

        UpdateJointPositions(endProgA, endProgB, endingProg);
        UpdateThumb();
        UpdateMaterials(endingProg, endProgB);
    }

    private void UpdateHover()
    {
        float hoverHighlight = PanelManager.Instance.HoveredSummoner == interaction ? 1 : 0;
        hoverness = Mathf.Lerp(hoverness, hoverHighlight, Time.deltaTime * 5);
        highlightMat.SetFloat("_Hover", hoverness);
    }

    private void UpdateThumb()
    {
        float thumbShownessTarget = interaction.State == SummonerState.Placing ? 0 : 1;
        thumbShowness = Mathf.Lerp(thumbShowness, thumbShownessTarget, Time.deltaTime * 5);
        Thumbnail.localScale = new Vector3(thumbShowness, thumbShowness, thumbShowness);
    }

    private void UpdateShowHide()
    {
        float shownessTarget = interaction.State == SummonerState.Hidden ? 0 : 1;
        showness = Mathf.Lerp(showness, shownessTarget, Time.deltaTime * 8);
        ElementsRoot.localScale = new Vector3(showness, showness, showness);
        ElementsRoot.gameObject.SetActive(showness > .1f);
    }

    private void UpdateHighlighting()
    {
        if(interaction.State == SummonerState.Placing)
        {
            highlightingDurationRemaining += Time.deltaTime;
        }
        else
        {
            highlightingDurationRemaining -= Time.deltaTime;
        }
        highlightingDurationRemaining = Mathf.Clamp(highlightingDurationRemaining, 0, PanelManager.Instance.HighlightingDuration);
    }

    private void UpdateJointPositions(float endProgA, float endProgB, float endingProg)
    {
        if(interaction.State == SummonerState.Ready)
        {
            InnerMeshEndJoint.localPosition = Vector3.Lerp(summonStartPoint.localPosition, summonEndPoint.localPosition, endProgA);
        }
        else
        {
            InnerMeshEndJoint.localPosition = Vector3.Lerp(summonStartPoint.localPosition, summonEndPoint.localPosition, interaction.SlideProgress);
        }
        InnerMeshStartJoint.localPosition = Vector3.Lerp(summonStartPoint.localPosition, summonEndPoint.localPosition, endProgA);
        OuterMeshStartJoint.localPosition = Vector3.Lerp(summonStartPoint.localPosition, summonEndPoint.localPosition, endingProg);

        float rotation = Mathf.Pow(endProgB, 5) * 45;
        OuterMeshStartJoint.localRotation = Quaternion.Euler(0, rotation, 0);
        OuterMeshEndJoint.localRotation = Quaternion.Euler(0, rotation, 0);

        float blendWeight = Mathf.Pow(endProgB, 3) * 100;
        OuterMeshRenderer.SetBlendShapeWeight(0, blendWeight);
    }

    private void UpdateMaterials(float endProgA, float endProgB)
    {
        float activenessTarget = interaction.State != SummonerState.Ready ? 1 : 0;
        activeness = Mathf.Lerp(activeness, activenessTarget, Time.deltaTime * 5);
        innerMeshMat.SetFloat("_Activeness", activeness);

        highlightMat.SetFloat("_HighlightProg", endProgA);
        outerMeshMat.SetFloat("_ArrowsAlpha", endProgB);
    }
}
