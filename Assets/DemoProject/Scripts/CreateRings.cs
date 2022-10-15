using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateRings : MonoBehaviour
{
    [SerializeField] private SelectRing _selectRing;
    [SerializeField] private List<Rings> _ringsList;

    private void OnEnable()
    {
        EventManager.OnLevelEndAction += LevelEndAction;
    }

    private void OnDisable()
    {
        EventManager.OnLevelEndAction -= LevelEndAction;
    }

    void LevelEndAction(int state)
    {
        if(state == 2){ResetGamaStats(); CreateGame();}
    }
    
    private void Awake()
    {
        ResetGamaStats();
        CreateGame();
    }
    
    public int openCount = 6;
    int rand;
    public int color1;
    
    void CreateGame()
    {
        for (int i = 0; i < openCount; i++)
        {
            rand = RandomNumberGenerator();
            if (_selectRing.ringNodes[rand].level == _selectRing.ringNodes[rand].ringDatas.Length) rand = 0;
            if (_selectRing.ringNodes[rand].level == _selectRing.ringNodes[rand].ringDatas.Length) rand = 1;
            if (_selectRing.ringNodes[rand].level == _selectRing.ringNodes[rand].ringDatas.Length) rand = 2;
            Debug.Log(rand);
            color1++;
            var node = _selectRing.ringNodes[rand];
            Debug.Log(_ringsList[rand].ringDataList[node.level].gameObject.name);
            var startPos = _ringsList[rand].ringDataList[node.level]._ringDataClass.startPosTransform.position;
            _ringsList[rand].ringDataList[node.level].transform.position = startPos;
            _ringsList[rand].ringDataList[node.level].gameObject.SetActive(true);
            _ringsList[rand].ringDataList[node.level]._ringDataClass.ringID = node.level;
            _ringsList[rand].ringDataList[node.level]._ringDataClass.nodeID = rand;
            if (color1 <= openCount / 2)
            {
                _ringsList[rand].ringDataList[node.level]._ringDataClass.ringColor = RingColors.RingColor.blue;//random olucak
                node.ringDatas[node.level].ringColor = RingColors.RingColor.blue;//random olucak
            }
            else
            {
                _ringsList[rand].ringDataList[node.level]._ringDataClass.ringColor = RingColors.RingColor.pink;//random olucak
                node.ringDatas[node.level].ringColor = RingColors.RingColor.pink;//random olucak
            }
            node.ringDatas[node.level].canSelect = true;
            if(node.level != 0) node.ringDatas[node.level - 1].canSelect = false;
            if (node.level == node.ringDatas.Length) node.canDrop = false;
            node.level++;
        }
        
        for (int i = 0; i < _selectRing.ringNodes.Count; i++)
        {
            if(_selectRing.ringNodes[i].level != _selectRing.ringNodes[i].ringDatas.Length) _selectRing.ringNodes[i].canDrop = true;
        }

        EventManager.RingDataSetAction();
    }
    
    void ResetGamaStats()
    {
        color1 = 0;
        for (int i = 0; i < _ringsList.Count; i++)
        {
            for (int j = 0; j < _ringsList[i].ringDataList.Count; j++)
            {
                _ringsList[i].ringDataList[j].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _selectRing.ringNodes.Count; i++)
        {
            _selectRing.ringNodes[i].canDrop = false;
            _selectRing.ringNodes[i].level = 0;
            for (int j = 0; j < _selectRing.ringNodes[i].ringDatas.Length; j++)
            {
                _selectRing.ringNodes[i].ringDatas[j].canSelect = false;
            }
        }
    }
    
    int RandomNumberGenerator()
    {
        return Random.Range(0, _selectRing.ringNodes.Count);
    }
    
    RingNode ReturnRing(RingPositionTypes.RingPositionType ringPositionType)
    {
        var node = _selectRing.ringNodes.Where(collectData => ringPositionType.Equals(collectData.ringType))
            .Select(x => x).FirstOrDefault();
        return node;
    }
    
    
    [Serializable]
    public class Rings
    {
        public RingPositionTypes.RingPositionType ringPositionType;
        public List<RingData> ringDataList;
        public Rings(RingPositionTypes.RingPositionType ringPositionType, List<RingData> ringDataList)
        {
            this.ringPositionType = ringPositionType;
            this.ringDataList = ringDataList;
        }
    }
}
