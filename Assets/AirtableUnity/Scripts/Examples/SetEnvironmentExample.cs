using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnvironmentExample : MonoBehaviour
{
    [Header("Script Configuration")] 
    public bool AutoStart = true;
    
    [Header("Environment Configuration")]
    public string ApiVersion;
    public string AppKey;
    public string ApiKey;
    
    // Start is called before the first frame update
    void Start()
    {
        if(AutoStart)
            SetEnvironment();
    }

    [ContextMenu("Set Environment")]
    public void SetEnvironment()
    {
        AirtableUnity.PX.Proxy.SetEnvironment(ApiVersion, AppKey, ApiKey);
    }
}
