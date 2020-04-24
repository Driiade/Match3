﻿using BC_Solution;
using System;
using TMPro;
using UnityEngine;


/// <summary>
/// Help to Debug current state for a statedMono service
/// </summary>
/// <typeparam name="T"></typeparam>
public class StatedMonoServiceDebugger<T> : MonoBehaviour where T  : Enum
{

    [SerializeField]
    TextMeshProUGUI text;

    StatedMono<T> statedMono;
    void Start()
    {
        statedMono = ServiceProvider.GetService<StatedMono<T>>();
        text.text = statedMono.CurrentStateType.ToString();
        statedMono.OnSwitchState += UpdateText;
    }

    void OnDestroy()
    {
        statedMono.OnSwitchState -= UpdateText;
    }

    void UpdateText(T stateType)
    {
        text.text = stateType.ToString();
    }

}