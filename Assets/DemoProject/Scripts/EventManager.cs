using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void RingDataSet();
    public static event RingDataSet OnRingDataSetAction;
    public static void RingDataSetAction()
    {
        OnRingDataSetAction?.Invoke();
    }
    
    public delegate void LevelEnd(int state);
    public static event LevelEnd OnLevelEndAction;
    public static void LevelEndAction(int state)
    {
        OnLevelEndAction?.Invoke(state);
    }
}
