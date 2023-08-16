using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JourneyTheme
    {
        ADVENTURE = 0,
        COLLECTION,
        ENVIRONMENT,
        TOUR,
    }
    public enum JourneyDifficulty
    {
        BEGINNER,
        INTERMEDIATE,
        EXPERT
    }
    public enum JourneyReward
    {
        EXP,
        CREDITS,
        ITEM
    }

    [CreateAssetMenu(menuName = "JourneyItems/Journey")]
    public class Journey : ScriptableObject
    {
        public Action<Journey> OnCompJourney;
        public Action<Mission> OnCompMission;
        
        public int Id;
        public JourneyTheme Theme;
        public string Title;
        [TextArea(10,20)] public string Description;
        public string DescriptionKey;
        public string Reward;
        public List<Mission> MissionList;
        public JourneyDifficulty Difficulty;

        public int MaxSequence => MissionList.Count;
        public int NowSequence => MissionListProgressId;
        
        private int MissionListProgressId { get; set; }
        
        public bool IsComplete => MissionListProgressId >= MissionList.Count;
        
        public Mission NowMission => MissionList[MissionListProgressId];

        public void Initialize(int seq)
        {
            MissionListProgressId = seq;
            for (int i = 0; i < MissionList.Count; i++)
            {
                MissionList[i].Initialize();
            }
        }

        public void Check(int stageID, int fishID, CatchAction action)
        {
            Mission mission = MissionList[MissionListProgressId];
            if (mission.Stage.id != stageID) return; 
            bool isMissionComp = mission.Check(fishID, action);

            if (isMissionComp == false) return;
            MissionListProgressId++;
            OnCompMission?.Invoke(mission);

            if (IsComplete == false) return;
            OnCompJourney?.Invoke(this);
        }
    }

// 레거시
// [CreateAssetMenu(menuName = "Journey")]
// public class Journey : ScriptableObject
// {
//     [SerializeField]
//     public JourneyTheme theme;
//     /*
//     ADVENTURE: 01
//     COLLECTION: 02
//     ENVIRONMENT: 03
//     TOUR : 04
//     */
//     public string id;
//     public string title;
//     public RegionType[] regionArray;
//     public string description;
//     public Sprite journeyImage;
//     public List<JourneyQuest> journeyQuestList;
//     public List<JourneyReward> rewardList;
//     public JourneyDifficulty journeyDifficulty;
// }