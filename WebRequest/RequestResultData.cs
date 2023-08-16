using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RequestResultData
{
    protected const string KEY_ERR_CODE = "err_code";
        
    public int ErrorCode;
        
    protected RequestResultData()
    {
            
    }
        
    protected RequestResultData(int errorCode)
    {
        ErrorCode = errorCode;
    }
}