using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class ChibiController : MonoBehaviour
{
    [SerializeField] private Animator _mainAnim;
    
    private void OnEnable()
    {
        EventManager.OnLevelEndAction += LevelEndAction;
    }

    private void OnDisable()
    {
        EventManager.OnLevelEndAction -= LevelEndAction;
    }
    
    private void Awake()
    {
        _mainAnim = GetComponent<Animator>();
    }

    void LevelEndAction(int state)
    {
        if (state == 1) TriggerAnimAction("Dance");
        else if (state == 2) TriggerAnimAction("Idle");
    }
    
    void TriggerAnimAction(string animName)
    {
        _mainAnim.SetTrigger(animName);
    }
}
