using System;
using System.Collections;
using AirtableUnity.PX.Model;
using UnityEngine;

public class DeleteRecord : MonoBehaviour
{
    [Header("Record Id")] 
    public string TableName;
    public string RecordId;
    
    /// <summary>
    /// Will delete a record using the data configured on Editor and without a custom callback
    /// </summary>
    [ContextMenu("Delete Record")]
    public void DeleteAirtableRecord()
    {
        DeleteAirtableRecord<BaseField>(TableName, RecordId, null);
    }
    
    /// <summary>
    /// Will delete a record using the data on paremeters with a custom callback
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="recordId"></param>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    public void DeleteAirtableRecord<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        StartCoroutine(DeleteRecordCo(tableName, recordId, callback));
    }

    private IEnumerator DeleteRecordCo<T>(string tableName, string recordId, Action<BaseRecord<T>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.DeleteRecordCo<T>(tableName, recordId, (recordDeleted) =>
        {
            OnResponseFinish(recordDeleted);
            callback?.Invoke(recordDeleted);
        }));
    }

    private static void OnResponseFinish<T>(BaseRecord<T> record)
    {
        var msg = "record id: " + record?.id + "\n";
        
        Debug.Log("[Airtable Unity] - Delete Record: " + "\n" + msg);
    }
}
