using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JourneyItems/Mission")]
public class Mission : ScriptableObject
{
    public StageItem Stage;
    public List<Target> TargetList = new List<Target>();
    public string Title;
    [TextArea(10,20)] public string Description;
    public string DescriptionKey;

    public bool IsComplete => TargetListProgressIdx >= TargetList.Count;
    public string NowProgressStr => $"{TargetListProgressIdx} / {TargetList.Count}";

    public Action<Target> OnCompTarget;

    private int TargetListProgressIdx { get; set; }
    public Target NowTarget => TargetList[TargetListProgressIdx];

    public void Initialize()
    {
        TargetListProgressIdx = 0;
    }
    
    public bool Check(int fishID, CatchAction action)
    {
        Target target = TargetList[TargetListProgressIdx];
        bool isTargetComp = target.Check(fishID, action);
        if (!isTargetComp) return IsComplete;
        
        TargetListProgressIdx++;
        OnCompTarget?.Invoke(target);
        return IsComplete;
    }
}