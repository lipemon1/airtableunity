using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;

public class GetRecordExample : MonoBehaviour
{
    [Header("Record Id")] 
    public string TableName;
    public string RecordId;
    
    [ContextMenu("Get Record")]
    public void GetRecord()
    {
        StartCoroutine(GetRecordCo());
    }

    private IEnumerator GetRecordCo()
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.GetRecordCo<BaseField>(TableName, RecordId, OnResponseFinish));
    }

    private void OnResponseFinish(BaseRecord<BaseField> record)
    {
        var msg = "record id: " + record?.id + "\n";
        msg += "field id: " + record?.fields?.Id + "\n";
        msg += "created at: " + record?.createdTime?.ToString();
        
        Debug.Log("[Airtable Unity] - Get Record: " + "\n" + msg);
    }
}
