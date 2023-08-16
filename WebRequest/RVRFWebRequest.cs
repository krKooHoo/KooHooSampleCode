using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Miragesoft.Utils.Json;
using Miragesoft.Utils.Web;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public static partial class RVRFWebRequest
{
    public class RequestResult<T>
    {
        public Action<T> OnComplete = null;
    }
        
    private const string STORE_TYPE_QUEST = "quest";
    private const string STORE_TYPE_RIFT = STORE_TYPE_QUEST;
    private const string STORE_TYPE_PICO_GLOBAL = "pico";
    private const string STORE_TYPE_PICO_CHINA = STORE_TYPE_PICO_GLOBAL;
            
    private static string GetMyStoreType => STORE_TYPE_QUEST;
}