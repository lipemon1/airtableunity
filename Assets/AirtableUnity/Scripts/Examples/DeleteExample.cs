using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;

public class DeleteExample : MonoBehaviour
{
    [Header("Record Id")] 
    public string TableName;
    public string RecordId;
    
    [ContextMenu("Delete Record")]
    public void DeleteRecord()
    {
        StartCoroutine(DeleteRecordCo());
    }

    private IEnumerator DeleteRecordCo()
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.DeleteRecordCo<BaseField>(TableName, RecordId, OnResponseFinish));
    }

    private void OnResponseFinish(BaseRecord<BaseField> record)
    {
        var msg = "record id: " + record?.id + "\n";
        
        Debug.Log("[Airtable Unity] - Delete Record: " + "\n" + msg);
    }
}
