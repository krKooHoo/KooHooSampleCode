using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JourneyItems/Target")]
public class Target : ScriptableObject
{
    [Header("FishItem에 AnyKindFish Item을 설정할 경우 종과 관계없이 완료 처리됨")]
    public FishItem Fish = null;
    public CatchAction CatchAction = CatchAction.KEEP;

    public bool Check(int fishID, CatchAction action)
    {
        //FishiItem의 ID가 -100일 경우 AnyKind Fish 판정
        bool checkFish = fishID == Fish.ID || Fish.ID == -100;
        bool checkAction = action == CatchAction;
        return checkFish && checkAction;
    }
}