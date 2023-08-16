using System;
using System.Collections;
using System.Collections.Generic;
using DUG.UI;
using Miragesoft.Utils.Json;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using JsonUtility = UnityEngine.JsonUtility;

public class JourneySystem : Singleton<JourneySystem>
{
    /*
     * Journey =
     * 1. Carp 수집가
     *  Mission =
     *  1.1 붕어 수집
     *      Target =
     *      1.1.1 붕어, 잡기
     *      1.1.2 붕어, 잡기
     *      1.1.3 붕어, 잡기
     *  1.2 잉어 수집,
     *      Target =
     *      1.2.1 잉어, 잡기
     *      1.2.2 잉어, 잡기
     *      1.2.3 잉어, 잡기
     *  1.3. 향어 수집
     *      Target =
     *      1.3.1 향어, 잡기
     *      1.3.2 향어, 잡기
     *      1.3.3 향어, 잡기
     */
    
    /* 호출 순서
     * OnStartJourney( Carp 수집가)
     * 
     * OnCompTarget(붕어, 잡기)
     * OnUpdatedMission(붕어 수집)
     * OnCompTarget(붕어, 잡기)
     * OnUpdatedMission(붕어 수집)
     * OnCompTarget(붕어, 잡기)
     * OnUpdatedMission(붕어 수집)
     * OnCompMission(붕어 수집)
     * 
     * OnCompTarget(잉어, 잡기)
     * OnUpdatedMission(잉어 수집)
     * OnCompTarget(잉어, 잡기)
     * OnUpdatedMission(잉어 수집)
     * OnCompTarget(잉어, 잡기)
     * OnUpdatedMission(잉어 수집)
     * OnCompMission(잉어 수집)
     * 
     * OnCompTarget(향어, 잡기)
     * OnUpdatedMission(잉어 수집)
     * OnCompTarget(향어, 잡기)
     * OnUpdatedMission(잉어 수집)
     * OnCompTarget(향어, 잡기)
     * OnUpdatedMission(잉어 수집)
     * OnUpdatedMission(향어 수집)
     * OnCompMission(향어 수집)
     * 
     * OnCompJourney(Carp 수집가)
     */
    
    private class LastJourneyInfo
    {
        public int ID { get; set; }
        public int Sequence { get; set; }
        public JourneyTheme Theme { get; set; }
    }

    private class DoneJourneyInfo
    {
        public List<int> DoneList = new List<int>();
    }
    
    public class Callbacks
    {
        public Action<Journey> OnStartJourney;
        public Action<Journey> OnQuitJourney;
        public Action<Journey> OnEndJourney;
        
        public Action<Target> OnCompTarget;
        
        public Action<Mission> OnUpdatedMission;
        public Action<Mission> OnCompMission;
        
        public Action<Journey> OnCompJourney;
    }
    
    public Callbacks Subscribe = new Callbacks();

    public List<Journey> JourneyDatabase;
    
    private Dictionary<JourneyTheme, List<Journey>> JourneyDic = new Dictionary<JourneyTheme, List<Journey>>();
    
    private LastJourneyInfo LastJourney;
    //private DoneJourneyInfo DoneJourney;
    
    public Journey NowJourney { get; private set; }

    public bool IsPlayingJourney => NowJourney != null;
    public Journey DummyJourney;

    private void Start()
    {
        for (int i = 0; i < JourneyDatabase.Count; i++)
        {
            var journey = JourneyDatabase[i];
            if (JourneyDic.ContainsKey(journey.Theme) == false)
            {
                JourneyDic.Add(journey.Theme, new List<Journey>());
            }

            var list = JourneyDic[journey.Theme];
            list.Add(journey);
        }
    }

    public void InitializeJourneySystem()
    {
        // 테스트용 코드
        Subscribe.OnStartJourney += journey => Debug.Log($"JourneySystem OnStartJourney ::: {journey.Title}");
        Subscribe.OnCompTarget += target => Debug.Log($"JourneySystem OnCompTarget ::: {target.Fish.ID}, {target.CatchAction.ToString()}");
        Subscribe.OnUpdatedMission += mission => Debug.Log($"JourneySystem OnUpdatedMission ::: {mission.Title}");
        Subscribe.OnCompMission += mission => Debug.Log($"JourneySystem OnCompMission ::: {mission.Title}, {NowJourney.NowSequence}");
        Subscribe.OnCompJourney += journey => Debug.Log($"JourneySystem OnCompJourney :::{journey.Title}");
        //

        Subscribe.OnCompTarget += target =>
        {
            if(NowJourney.NowMission.IsComplete) return;
            string title = NowJourney.Title;
            string content = $"{NowJourney.NowMission.Title} : {NowJourney.NowMission.NowProgressStr}";

            NotificationManager.Instance.ShowNotificationPopup(title, content, 3.0f);
        };
        
        Subscribe.OnCompMission += target =>
        {
            if (NowJourney.IsComplete) return;
            if (NowJourney.NowMission.IsComplete) return;
            
            string title = NowJourney.Title;
            string[] content = { $"{NowJourney.NowMission.Title} Complete!", "Go To Lodge" };
            
            //NotificationManager.Instance.ShowNotificationPopup(title, content, 3.0f);

            Action<DUGButtonEventArgs> action = args =>
            {
                staticLoading.LoadScene("Lobby");
            };

            NotificationManager.Instance.ShowNotificationButtonPopup(title, content,
                NotificationManager.NotificationPos.Middle, action,
                NotificationManager.NotificationLocalization.NoLocalization);
        };
        
        Subscribe.OnCompJourney += target =>
        {
            string title = NowJourney.Title;
            string[] content = { $"{NowJourney.Title} Journey Complete!", "Go To Lodge" };
            
            Action<DUGButtonEventArgs> action = args =>
            {
                staticLoading.LoadScene("Lobby");
            };

            NotificationManager.Instance.ShowNotificationButtonPopup(title, content,
                NotificationManager.NotificationPos.Middle, action,
                NotificationManager.NotificationLocalization.NoLocalization);
        };

        Subscribe.OnEndJourney += journey =>
        {
            int goldValue = 1000;
            string prevUserGold = DataManager.Instance.userData.userGold.ToString();

            DataManager.Instance.AddGold(goldValue);

            string[] popupContens = new string[]
            {
                $"{prevUserGold}", $"+ {(goldValue)}",
                $"Your Credits {DataManager.Instance.userData.userGold}"
            };

            Action<DUGButtonEventArgs> loading = (eventArgs) =>
            {
                staticLoading.Stage = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("Loading");
            };

            NotificationManager.Instance.ShowNotificationButtonPopup("Journey Reward", popupContens,
                NotificationManager.NotificationPos.Middle, loading,
                NotificationManager.NotificationLocalization.NoLocalization);
        };

        
        {
            string lastJourneyData = NewSystems.FileIO.LoadEncryptedTextFromFile("LastJourney");
        
            if (string.IsNullOrEmpty(lastJourneyData) == false)
            {
                var lastJourneyInfo = JsonConvert.DeserializeObject<LastJourneyInfo>(lastJourneyData);
                if (lastJourneyInfo.ID != -1)
                {
                    Journey lastJourney = JourneyDic[lastJourneyInfo.Theme].Find(journey => journey.Id == lastJourneyInfo.ID);
                    StartJourney(lastJourney, lastJourneyInfo.Sequence);
                }
            }
            
            Debug.Log($"JourneySystem ::: {lastJourneyData}");
        }
      
        // {
        //     string doneJourneyData = NewSystems.FileIO.LoadEncryptedTextFromFile("DoneJourney");
        //
        //     if (string.IsNullOrEmpty(doneJourneyData) == false)
        //     {
        //         DoneJourney = JsonConvert.DeserializeObject<DoneJourneyInfo>(doneJourneyData);
        //     }
        //     
        //     Debug.Log($"JourneySystem DoneJourney ::: {DoneJourney.ToJson().ToString()}");
        // }
    }

    private void SaveJourney()
    {
        LastJourneyInfo info = new LastJourneyInfo();
        info.ID = IsPlayingJourney ? NowJourney.Id : -1;
        info.Sequence = IsPlayingJourney ? NowJourney.NowSequence : 0;
        info.Theme = IsPlayingJourney ? NowJourney.Theme : JourneyTheme.ADVENTURE ;
        string data = info.ToJson().ToString();
        NewSystems.FileIO.SaveEncryptedTextToFile("LastJourney",data);

        // string doneJourneyData = DoneJourney.ToJson().ToString();
        // NewSystems.FileIO.SaveEncryptedTextToFile("DoneJourney",doneJourneyData);
        
        //Debug.Log($"JourneySystem ::: LastJourney : {data}, DoneJourney : {doneJourneyData}");
    }

#if UNITY_EDITOR

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            int stageID = NowJourney.NowMission.Stage.id;
            Target param = NowJourney.NowMission.NowTarget;
            Check(stageID, param.Fish.ID,param.CatchAction);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            EndJourney();
        }
    }
    
#endif

    /// <summary>
    /// JourneyDatabase에 해당 Theme 없을시 Error
    /// </summary>
    /// <param name="theme"></param>
    /// <returns></returns>
    public List<Journey> GetJourneyListByTheme(JourneyTheme theme)
    {
        return JourneyDic[theme];
    }

    public void StartJourney(Journey journey, int seq = 0)
    {
        journey.Initialize(seq);
        NowJourney = journey;
        NowJourney.NowMission.OnCompTarget = OnCompTarget;
        NowJourney.OnCompMission = OnCompMission;
        NowJourney.OnCompJourney = OnCompletedJourney;

        SaveJourney();
        Subscribe.OnStartJourney?.Invoke(NowJourney);
    }

    public void QuitJourney()
    {
        Journey journey = NowJourney;
        NowJourney = null;
        Subscribe.OnQuitJourney?.Invoke(journey);
    }
    
    public void EndJourney()
    {
        Journey journey = NowJourney;
        NowJourney = null;
        Subscribe.OnEndJourney?.Invoke(journey);
    }

    private void OnCompTarget(Target target)
    {
        //미션 업데이트는 여기에 있음
        
        Subscribe.OnCompTarget?.Invoke(target);
        Subscribe.OnUpdatedMission?.Invoke(NowJourney.NowMission);
    }
    
    private void OnCompMission(Mission mission)
    {
        Subscribe.OnCompMission?.Invoke(mission);
        SaveJourney();
        //DataManager.Instance.IncreaseSaveCountAndSave();
        
        if(NowJourney.IsComplete == false)
            NowJourney.NowMission.OnCompTarget = OnCompTarget;
    }
    
    private void OnCompletedJourney(Journey journey)
    {
        Subscribe.OnCompJourney?.Invoke(NowJourney);
        //DoneJourney.DoneList.Add(journey.Id);
        SaveJourney();
    }
    
    public void Check(int stageID, int fishID, CatchAction action)
    {
        if (NowJourney == null)
        {
            Debug.LogError("JourneySystem ::: NowJourney is Null");
            return;
        }
        
        NowJourney.Check(stageID, fishID, action);
    }

   
}


