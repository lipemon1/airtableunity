using System;
using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using Newtonsoft.Json;
using UnityEngine;

public class CreateRecord : MonoBehaviour
{
    [Header("Table Name")] 
    public string TableName;
    [TextArea(0,10)]
    public string NewRecordJson;
    
    /// <summary>
    /// Will create a record using the data inside Editor without any callback
    /// IMPORTANT - newData must be encapsulated inside a object called 'fields' if you dont know how to
    /// do that use the overload when you pass the object it self
    /// </summary>
    [ContextMenu("Create Record")]
    public void CreateAirtableRecord()
    {
        CreateAirtableRecord<BaseField>(TableName, NewRecordJson, null);
    }

    /// <summary>
    /// Will create a new record using the data passed by paremeters
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="newData"></param>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    public void CreateAirtableRecord<T>(string tableName, BaseRecord<T> newData, Action<BaseRecord<T>> callback)
    {
        var wrapper = new
        {
            fields = newData.fields
        };
        
        StartCoroutine(CreateRecordCo(tableName, JsonConvert.SerializeObject(wrapper), callback));
    }

    private void CreateAirtableRecord<T>(string tableName, string newData, Action<BaseRecord<T>> callback)
    {
        StartCoroutine(CreateRecordCo(tableName, newData, callback));
    }

    private IEnumerator CreateRecordCo<T>(string tableName, string newData, Action<BaseRecord<T>> callback)
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.CreateRecordCo<T>(tableName, newData, (createdRecord) =>
        {
            OnResponseFinish(createdRecord);
            callback?.Invoke(createdRecord);
        }));
    }

    private static void OnResponseFinish<T>(BaseRecord<T> record)
    {
        var msg = "record id: " + record?.id + "\n";
        msg += "created at: " + record?.createdTime;
        
        Debug.Log("[Airtable Unity] - Create Record: " + "\n" + msg);
    }
}
