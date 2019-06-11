using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;

public class ListRecordsExample : MonoBehaviour
{
    [Header("Table Name")] 
    public string TableName;

    [ContextMenu("Get Table Records")]
    public void GetTableRecords()
    {
        StartCoroutine(GetTableRecordsCo());
    }

    private IEnumerator GetTableRecordsCo()
    {
        yield return StartCoroutine(AirtableUnity.PX.Proxy.ListRecordsCo<BaseField>(TableName, OnResponseFinish));
    }

    private void OnResponseFinish(List<BaseRecord<BaseField>> records)
    {
        Debug.Log("[Airtable Unity] - List Records: " + records?.Count);
    }
}
