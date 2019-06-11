using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;

public class UpdateRecordExample : MonoBehaviour
{
    [Header("Record Configuration")] 
    public string TableName;
    public string RecordId;
    [TextArea(0,10)]
    public string NewRecordJson;
    
    [ContextMenu("Update Record")]
    public void UpdateRecord()
    {
        StartCoroutine(GetRecordCo());
    }
    
    [ContextMenu("Hard Update Record")]
    public void HardUpdateRecord()
    {
        StartCoroutine(GetRecordCo(true));
    }

    private IEnumerator GetRecordCo(bool useHardUpdate = false)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.UpdateRecordCo<BaseField>(TableName, RecordId, NewRecordJson, OnResponseFinish, useHardUpdate));
    }

    private void OnResponseFinish(BaseRecord<BaseField> record)
    {
        var msg = "record id: " + record?.id + "\n";
        msg += "field id: " + record?.fields?.Id + "\n";
        msg += "created at: " + record?.createdTime?.ToString();
        
        Debug.Log("[Airtable Unity] - Update Record: " + "\n" + msg);
    }
}
