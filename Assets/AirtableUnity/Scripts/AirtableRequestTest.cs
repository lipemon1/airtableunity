using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirtableRequestTest : MonoBehaviour {
    public string ApiVersion;
    public string AppKey;
    public string ApiKey;

    public string TableToTest;
	// Use this for initialization
	void Start () {
        StartCoroutine(CallTableTest());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator CallTableTest()
    {
        // Get UserMe
        yield return AirtableUnity.PX.Proxy.GetRecordsFromTable(TableToTest).SendWebRequest(
            (response) =>
            {
                Debug.Log(response.Message);
            });
    }
}
