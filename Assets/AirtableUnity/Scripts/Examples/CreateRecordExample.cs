using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;

public class CreateRecordExample : MonoBehaviour
{
    [Header("Table Name")] 
    public string TableName;
    [TextArea(0,10)]
    public string NewRecordJson;
    
    [ContextMenu("Create Record")]
    public void CreateRecord()
    {
        StartCoroutine(CreateRecordCo());
    }

    private IEnumerator CreateRecordCo()
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.CreateRecordCo<BaseField>(TableName, NewRecordJson, OnResponseFinish));
    }

    private void OnResponseFinish(BaseRecord<BaseField> record)
    {
        var msg = "record id: " + record?.id + "\n";
        msg += "field id: " + record?.fields?.Id + "\n";
        msg += "created at: " + record?.createdTime?.ToString();
        
        Debug.Log("[Airtable Unity] - Create Record: " + "\n" + msg);
    }
}
