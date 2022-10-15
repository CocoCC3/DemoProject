using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enums;

public class RingData : MonoBehaviour
{
    [SerializeField] private GhostColorSO _ghostColorSO;
    [SerializeField] private MeshRenderer _mainMeshRenderer;
    [Space]
    [Header("RingData")]
    [SerializeField] public RingDataClass _ringDataClass;

    private void OnEnable()
    {
        EventManager.OnRingDataSetAction += RingDataSetAction;
    }

    private void OnDisable()
    {
        EventManager.OnRingDataSetAction -= RingDataSetAction;
    }

    void RingDataSetAction()
    {
        ColorSet();
    }
    
    void ColorSet()
    {
        var colorData = ColorRingData(_ringDataClass.ringColor);
        _mainMeshRenderer.material = colorData.mainRingMat;
    }
    
    ColorToRing ColorRingData(RingColors.RingColor ringColor)
    {
        var data = _ghostColorSO._colorToRings.Where(collectData => ringColor.Equals(collectData.ringColor))
            .Select(x => x).FirstOrDefault();
        return data;
    }
    
    [Serializable]
    public class RingDataClass
    {
        public RingColors.RingColor ringColor;
        public Transform startPosTransform;
        public int nodeID;
        public int ringID;
        public RingDataClass(RingColors.RingColor ringColor, int nodeID, int ringID, Transform startPosTransform)
        {
            this.startPosTransform = startPosTransform;
            this.ringColor = ringColor;
            this.nodeID = nodeID;
            this.ringID = ringID;
        }
    }
}
