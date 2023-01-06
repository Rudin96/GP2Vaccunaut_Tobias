using System;
using System.Collections;
using System.Collections.Generic;
using Team14;
using UnityEngine;

[CreateAssetMenu(fileName = "new CounterTask", menuName = "ScriptableTasks/CounterTask", order = 0)]
public class ScriptableTaskCounter : ScriptableTaskBase
{
    public int AmountForTrigger;

    public int _counter;
    
    public override void Update()
    {
        if(_counter < AmountForTrigger)
            _counter++;
        RaiseUI();
        if (_counter == AmountForTrigger && (TaskStatus != TaskStatus.Completed || TaskStatus != TaskStatus.Inactive))
            Finish();
    }

    public override void ResetCounter()
    {
        _counter = 0;
    }

    public override string GetProgressText()
    {
        return $"({_counter}/{AmountForTrigger})";
    }
}
