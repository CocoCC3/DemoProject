using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Enums;

public class SelectRing : MonoBehaviour
{
    private bool _takeInput;
    public Vector3 _mousePosition;
    private Vector3 _smoothMousePos;
    private Plane plane;
    
    public Transform currentRing;
    public RingData.RingDataClass _currentRingData;
    private RingNode _selectedRingNode;
    private RingNode _lastRingNode;
    public List<GhostRing> _ghostRings;
    private bool _canPlay;
    
    private void Awake()
    {
        //NodeDataSet();
        _canPlay = true;
    }
    
    void NodeDataSet()
    {
        for (int i = 0; i < ringNodes.Count; i++)
        {
            if (ringNodes[i].level == ringNodes[i].ringDatas.Length) ringNodes[i].canDrop = false;
            else ringNodes[i].canDrop = true;
            for (int j = 0; j < ringNodes[i].ringDatas.Length; j++)
            {
                ringNodes[i].ringDatas[j].canSelect = false;
            }
            for (int j = 0; j < ringNodes[i].ringDatas.Length; j++)
            {
                ringNodes[i].ringDatas[ringNodes[i].level - 1].canSelect = true;
            }
        }
    }
    
    private void Start()
    {
        plane = new Plane(Vector3.up, transform.position);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){ _ghostRings.Clear(); _currentRingData = null; _takeInput = true;}
        if (Input.GetMouseButtonUp(0)) _takeInput = false;
        
        if (_takeInput && currentRing == null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Ring") {
                    var ringData = hit.transform.GetComponent<RingData>()._ringDataClass;
                    var selectedRingNode = ReturnStorage(ringData.nodeID);
                    if (selectedRingNode.ringDatas[ringData.ringID].canSelect) {
                        _currentRingData = ringData;
                        currentRing = hit.transform;
                        _lastRingNode = selectedRingNode;
                        selectedRingNode.level--;
                        NodeDropStatueSet(selectedRingNode);
                        selectedRingNode.ringDatas[selectedRingNode.level].canSelect = false;
                        if(selectedRingNode.level != 0) selectedRingNode.ringDatas[selectedRingNode.level - 1].canSelect = true;
                        GhostRingFinder(_currentRingData);
                        Debug.Log("Select Ring is: " + currentRing.gameObject.name);
                    }
                }
            }
        }
        
        if (currentRing != null) {GetMousePosForSelectedRing(currentRing);}
    }
    
    void GetMousePosForSelectedRing(Transform ring)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var enter))
        {
            _mousePosition = ray.GetPoint(enter);
            _smoothMousePos = _mousePosition;
            _mousePosition.y = 0.5f;
            _mousePosition = Vector3Int.RoundToInt(_mousePosition);
            _mousePosition.z = 0f;
            //ring.position = new Vector3(ring.position.x, ring.position.y, 0f);
            ring.position = _smoothMousePos + new Vector3(0f, 6f, 0f);//follow mouse pos
            ring.position = new Vector3(ring.position.x, ring.position.y, 0f);
            foreach (var node in ringNodes)
            {
                foreach (var nearPoss in node.nearCellPos)
                {
                    if (Vector3Int.RoundToInt(nearPoss) == _mousePosition && node.canDrop)
                    {
                        if (Input.GetMouseButtonUp(0) && currentRing != null)
                        {
                            GhostRingClose();
                            _selectedRingNode = node;
                            _takeInput = false;
                            if (currentRing == null) return;
                            if (!NodeIsFullStatueCheck(_selectedRingNode)){ _selectedRingNode = null; return;}
                            Debug.Log(_selectedRingNode.ringDatas[_selectedRingNode.level].ringTransforms);
                            _selectedRingNode.ringDatas[_selectedRingNode.level].ringColor = _currentRingData.ringColor;
                            _currentRingData.nodeID = _selectedRingNode.gridID;
                            _currentRingData.ringID = _selectedRingNode.level;
                            var jumpPos = _selectedRingNode.ringDatas[_selectedRingNode.level].ringTransforms.position;
                            currentRing.transform.DOLocalJump(jumpPos, 1f, 1, 0.3f).OnComplete(() => currentRing.position = jumpPos);
                            _selectedRingNode.ringDatas[_selectedRingNode.level].canSelect = true; 
                            if (_selectedRingNode.level != 0) { _selectedRingNode.ringDatas[_selectedRingNode.level - 1].canSelect = false;} 
                            _selectedRingNode.level++; NodeDropStatueSet(_selectedRingNode);
                            currentRing = null;
                            _selectedRingNode = null;
                            _lastRingNode = null;
                        }
                    }
                }
            }
            
            foreach (var node in ringNodes)
            {
                foreach (var nearPoss in node.nearCellPos)
                {
                    if (Vector3Int.RoundToInt(nearPoss) != _mousePosition)
                    {
                        DropLastIndex();
                    }
                    else if (Vector3Int.RoundToInt(nearPoss) != _mousePosition && !node.canDrop)
                    {
                        DropLastIndex();
                    }
                    else if (Vector3Int.RoundToInt(nearPoss) == _mousePosition && !node.canDrop)
                    {
                        DropLastIndex();
                    }
                }
            }
        }
    }

    void DropLastIndex()
    {
        if (Input.GetMouseButtonUp(0) && currentRing != null)
        {
            Debug.Log(_lastRingNode.ringDatas[_lastRingNode.level].ringTransforms + "!!!!!!!????????");
            GhostRingClose();
            var jumpPos = _lastRingNode.ringDatas[_lastRingNode.level].ringTransforms.position;
            currentRing.transform.DOLocalJump(jumpPos, 1f, 1, 0.3f).OnComplete(() => currentRing.position = jumpPos);
            _lastRingNode.ringDatas[_lastRingNode.level].canSelect = true;
            if(_lastRingNode.level != 0) _lastRingNode.ringDatas[_lastRingNode.level - 1].canSelect = false;
            _lastRingNode.level++;
            currentRing = null;
            NodeDropStatueSet(_lastRingNode);
        }
    }
    
    
    
    
    void NodeDropStatueSet(RingNode ringNode)
    {
        if (ringNode.level == ringNode.ringDatas.Length){ ringNode.canDrop = false; CheckForSameColor(ringNode);}
        else ringNode.canDrop = true;
    }

    private int winCount;
    private int winCountForNode;
    void CheckForSameColor(RingNode ringNode)
    {
        winCountForNode = 0;
        var color = ringNode.ringDatas[0].ringColor;
        for (int i = 0; i < ringNode.ringDatas.Length; i++)
        {
            if (ringNode.ringDatas[i].ringColor == color)
            {
                winCountForNode++;
                if (winCountForNode == ringNode.ringDatas.Length)
                {
                    Debug.Log("nodedone");
                    winCount++;
                    if (winCount == 2) {
                        winCount = 0;
                        winCountForNode = 0;
                        StartCoroutine(LevelEndDelay());
                    }
                }
            }
        }
    }
    
    IEnumerator LevelEndDelay()
    {
        Debug.Log("LEVEL-END");
        EventManager.LevelEndAction(1);
        yield return new WaitForSeconds(2f);
        EventManager.LevelEndAction(2); 
    }
    
    bool NodeIsFullStatueCheck(RingNode ringNode)
    {
        if (ringNode.canDrop){ return true;}
        else { return false;}
    }
    
    void GhostRingFinder(RingData.RingDataClass ringDataClass)
    {
        for (int i = 0; i < ringNodes.Count; i++)
        {
            if (ringNodes[i].canDrop)
            {
                for (int j = 0; j < ringNodes[i].ringDatas.Length; j++)
                {
                    Debug.Log(ringNodes[i].ringDatas[j].canSelect + ringDataClass.ringColor.ToString() + ringNodes[i].ringDatas[j].ringColor.ToString());
                    if (ringNodes[i].ringDatas[j].canSelect && ringDataClass.ringColor == ringNodes[i].ringDatas[j].ringColor)
                    {
                        var ghostRingObj = ringNodes[i].ringDatas[j + 1].ringTransforms.GetComponent<GhostRing>();
                        ghostRingObj.SetTheGhostRing(ringDataClass.ringColor, true);
                        _ghostRings.Add(ghostRingObj);
                    }
                }
            }
        }
    }
    
    void GhostRingClose()
    {
        for (int i = 0; i < _ghostRings.Count; i++)
        {
            _ghostRings[i].SetTheGhostRing(RingColors.RingColor.blue, false);//for close the hologram
        }
    }
    
    RingNode ReturnStorage(int ringID)
    {
        var node = ringNodes.Where(collectData => ringID.Equals(collectData.gridID))
            .Select(x => x).FirstOrDefault();
        return node;
    }
    
    [Space]
    [Header("RingNodeList")]
    [SerializeField] public List<RingNode> ringNodes;
    
}

[Serializable]
public class RingNode
{
    public RingPositionTypes.RingPositionType ringType;
    public int level;
    public RingDatas[] ringDatas;
    public Vector3 cellPos;
    public Vector3[] nearCellPos;
    public bool canDrop;
    public int gridID;
    public RingNode(Vector3 cellPos, bool canDrop ,int gridID, RingDatas[] ringDatas, int level, Vector3[] nearCellPos, RingPositionTypes.RingPositionType ringType)
    {
        this.nearCellPos = nearCellPos;
        this.ringDatas = ringDatas;
        this.ringType = ringType;
        this.cellPos = cellPos;
        this.canDrop = canDrop;
        this.gridID = gridID;
        this.level = level;
    }
}
    
[Serializable]
public class RingDatas
{
    public RingColors.RingColor ringColor;
    public Transform ringTransforms;
    public bool canSelect;
    public RingDatas(RingColors.RingColor ringColor, Transform ringTransforms, bool canSelect)
    {
        this.ringTransforms = ringTransforms;
        this.canSelect = canSelect;
        this.ringColor = ringColor;
    }
}
