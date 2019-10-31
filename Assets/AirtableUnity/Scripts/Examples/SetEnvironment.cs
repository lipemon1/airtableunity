using UnityEngine;

public class SetEnvironment : MonoBehaviour
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
            PrepareEnvironment();
    }

    /// <summary>
    /// Will prepare Airtable configuration using the Editor values
    /// </summary>
    [ContextMenu("Prepare Environment")]
    public void PrepareEnvironment()
    {
        AirtableUnity.PX.Proxy.SetEnvironment(ApiVersion, AppKey, ApiKey);
    }

    /// <summary>
    /// Will prepare Airtable configuration using parameter values
    /// </summary>
    /// <param name="apiVersion"></param>
    /// <param name="appKey"></param>
    /// <param name="apiKey"></param>
    public void PrepareEnvironment(string apiVersion, string appKey, string apiKey)
    {
        AirtableUnity.PX.Proxy.SetEnvironment(apiVersion, appKey, apiKey);
    }
}
