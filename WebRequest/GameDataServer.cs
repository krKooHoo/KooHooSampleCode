using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Miragesoft.Utils.Json;
using Miragesoft.Utils.Web;
using Newtonsoft.Json.Linq;
using RVRF.WebRequest;
using UnityEngine;
using UnityEngine.Networking;

public static class GameDataServer
{
        //게임 데이터 서버
        private const string URI_GAME_DATA_SERVER_TEST = @"https://test.koohoo.com";
    
        private static string GetGameDataServerUri => URI_GAME_DATA_SERVER_TEST;
        
        //게임 데이터 서버
        private static string GameDataCommonURI = $"{GetGameDataServerUri}/testStore";
        
        private static string GetVersionUri => $"{GameDataCommonURI}/getVersion";
        private static string GetUserDataUri => $"{GameDataCommonURI}/getUserData";
    
        private static async UniTask<string> RequestAsync(UnityWebRequest req)
        {
            UnityWebRequest op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        
        public static RVRFWebRequest.RequestResult<string> GetUserData(ulong id)
        {
            RVRFWebRequest.RequestResult<string> reVal = new RVRFWebRequest.RequestResult<string>();
    
            WWWForm form = new WWWForm();
            form.AddField("id", id.ToString());
            
            UnityWebRequest request = UnityWebRequest.Post(GetUserDataUri, form);
            request.AddAllowCrossDomainHeader();
    
            UniTask<string> task = RequestAsync(request);
            task.GetAwaiter().OnCompleted(() =>
            {
                string result = task.GetAwaiter().GetResult();
                reVal.OnComplete?.Invoke(result);
                request.Dispose();
            });
    
            return reVal;
        }
        
        public static RVRFWebRequest.RequestResult<string> GetVersion()
        {
            RVRFWebRequest.RequestResult<string> reVal = new RVRFWebRequest.RequestResult<string>();
    
            WWWForm form = new WWWForm();
            UnityWebRequest request = UnityWebRequest.Post(GetVersionUri, form);
            request.AddAllowCrossDomainHeader();
    
            UniTask<string> task = RequestAsync(request);
            task.GetAwaiter().OnCompleted(() =>
            {
                string result = task.GetAwaiter().GetResult();
                reVal.OnComplete?.Invoke(result);
                request.Dispose();
            });
    
            return reVal;
        }

}



