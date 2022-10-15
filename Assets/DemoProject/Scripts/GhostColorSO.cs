using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "GhostColorData", menuName = "ScriptableObjects/GhostColorSO", order = 1)]
public class GhostColorSO : ScriptableObject
{
    [Header("ColorToRingDataList")]
    [SerializeField] public List<ColorToRing> _colorToRings;
}

[Serializable]
public class ColorToRing
{
    public RingColors.RingColor ringColor;
    public Material ghostRingMat;
    public Material mainRingMat;
    public ColorToRing(RingColors.RingColor ringColor, Material mainRingMat, Material ghostRingMat)
    {
        this.ghostRingMat = ghostRingMat;
        this.mainRingMat = mainRingMat;
        this.ringColor = ringColor;
    }
}
