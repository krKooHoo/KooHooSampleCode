using System;
using System.Collections;
using System.Collections.Generic;
using Miragesoft.Utils.Json;
using UnityEngine;

using RVRF.WebRequest;
using MessageServer = RVRF.WebRequest.RVRFWebRequest.MessageServer;
using GameDataServer = RVRF.WebRequest.RVRFWebRequest.GameDataServer;
using StatisticsServer = RVRF.WebRequest.RVRFWebRequest.StatisticsServer;


public class WebRequestTester : MonoBehaviour
{
    void Update()
    {
        //WebRequest 호출 테스트 클래스.
        //아래의 방식들로 호출하면 됨.
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameDataServer.GetVersion().OnComplete += s =>
            {
                Debug.Log($"GetVersion ::: length = {s.Length} ||| {s}");
            };
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameDataServer.GetUserData(999999999).OnComplete += s =>
            {
                Debug.Log($"GetUserData ::: length = {s.Length} ||| {s}");
            };
        }
    }
}
