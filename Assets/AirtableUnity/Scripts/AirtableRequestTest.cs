using System.Collections;
using System.Collections.Generic;
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
	    yield return StartCoroutine(AirtableUnity.PX.Proxy.GetRecordsFromTable(TableToTest, ShowResponse));
    }

    private void ShowResponse(string response)
    {
	    Debug.Log(response);
    }
}
