using System;
using System.Collections;
using AirtableUnity.PX.Model;
using Newtonsoft.Json;
using UnityEngine;

public class UpdateRecordExample : MonoBehaviour
{
    [Header("Record Configuration")] 
    public string TableName;
    public string RecordId;
    [TextArea(0,10)]
    public string NewRecordJson;

    /// <summary>
    /// Will update a record using the Editor values and without a custom callback
    /// IMPORTANT - NewRecordJson must be encapsulated inside a object called 'fields' if you dont know how to
    /// do that use the overload when you pass the object it self
    /// </summary>
    [ContextMenu("Update Record")]
    public void UpdateAirtableRecord()
    {
        UpdateAirtableRecord<BaseField>(TableName, RecordId, NewRecordJson, null, false);
    }

    /// <summary>
    /// Will update a record using the paremeters passed and without a custom callback
    /// IMPORTANT - newData must be encapsulated inside a object called 'fields' if you dont know how to
    /// do that use the overload when you pass the object it self
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="recordId"></param>
    /// <param name="newData"></param>
    public void UpdateAirtableRecord<T>(string tableName, string recordId, string newData, Action<BaseRecord<T>> callback)
    { 
        UpdateAirtableRecord(tableName, recordId, newData, callback, false);
    }
    
    /// <summary>
    /// Will update a record using the paremeters passed and the callback, pass HardUpdate to use put instead of patch
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="recordId"></param>
    /// <param name="dataObject"></param>
    /// <param name="callback"></param>
    /// <param name="hardUpdate"></param>
    /// <typeparam name="T"></typeparam>
    public void UpdateAirtableRecord<T>(string tableName, string recordId, BaseRecord<T> dataObject, Action<BaseRecord<T>> callback, bool hardUpdate)
    {
        var wrapper = new
        {
            fields = dataObject.fields
        };
        
        UpdateAirtableRecord(tableName, recordId,JsonConvert.SerializeObject(wrapper), callback);
    }
    
    private void UpdateAirtableRecord<T>(string tableName, string recordId, string newData, Action<BaseRecord<T>> callback, bool hardUpdate = false)
    {
        StartCoroutine(GetRecordCo(tableName, recordId,newData, callback));
    }

    private IEnumerator GetRecordCo<T>(string tableName, string recordId, string newData, Action<BaseRecord<T>> callback, bool useHardUpdate = false)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.UpdateRecordCo<T>(tableName, recordId, newData,
            (baseRecordUpdated) =>
            {
                OnResponseFinish(baseRecordUpdated);
                callback?.Invoke(baseRecordUpdated);
            }, useHardUpdate));
    }

    private static void OnResponseFinish<T>(BaseRecord<T> record)
    {
        var msg = "record id: " + record?.id + "\n";
        msg += "created at: " + record?.createdTime;
        
        Debug.Log("[Airtable Unity] - Update Record: " + "\n" + msg);
    }
}
