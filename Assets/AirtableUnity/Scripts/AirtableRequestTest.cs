using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;

public class AirtableRequestTest : MonoBehaviour {
	[Header("Environment Configuration")]
    public string ApiVersion;
    public string AppKey;
    public string ApiKey;

    [Header("Other Settings")]
    public string TableToTest;
	// Use this for initialization
	void Start () {
        AirtableUnity.PX.Proxy.SetEnvironment(ApiVersion, AppKey, ApiKey);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[ContextMenu("Test Call")]
	public void TestCall()
	{
		StartCoroutine(CallTableTest());
	}

    private IEnumerator CallTableTest()
    {
	    yield return StartCoroutine(AirtableUnity.PX.Proxy.ListRecordsCo<BaseField>(TableToTest, ShowResponse));
    }

    private void ShowResponse(List<BaseRecord<BaseField>> records)
    {
	    Debug.Log(records?.Count);
    }
}
