using BC_Solution;
using System;
using TMPro;
using UnityEngine;


/// <summary>
/// Help to Debug current state for a statedMono service
/// </summary>
/// <typeparam name="T"></typeparam>
public class StatedMonoServiceDebugger<T> : MonoBehaviour, IAwakable where T  : Enum
{

    [SerializeField]
    TextMeshProUGUI text;

    StatedMono<T> statedMono;
    public void IAwake()
    {
        statedMono = ServiceProvider.GetService<StatedMono<T>>();
        text.text = statedMono.CurrentStateType.ToString();
        statedMono.OnEnterState += UpdateText;
    }

    void OnDestroy()
    {
        if(statedMono)
            statedMono.OnEnterState -= UpdateText;
    }

    void UpdateText(T stateType)
    {
        text.text = stateType.ToString();
    }

}
