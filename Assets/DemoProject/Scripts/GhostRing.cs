using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

public class GhostRing : MonoBehaviour
{
    [SerializeField] private GhostColorSO _ghostColorSO;
    [SerializeField] private MeshRenderer _ghostRingMeshRenderer;
    
    public void SetTheGhostRing(RingColors.RingColor ringColor, bool statue)
    {
        Debug.Log(statue + gameObject.name);
        var colorData = ColorRingData(ringColor);
        _ghostRingMeshRenderer.material = colorData.ghostRingMat;
        _ghostRingMeshRenderer.enabled = statue;
    }
    
    ColorToRing ColorRingData(RingColors.RingColor ringColor)
    {
        var data = _ghostColorSO._colorToRings.Where(collectData => ringColor.Equals(collectData.ringColor))
            .Select(x => x).FirstOrDefault();
        return data;
    }
}
