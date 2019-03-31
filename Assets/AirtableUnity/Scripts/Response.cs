using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirtableUnity.PX
{
    [Serializable]
    public class Response
    {
        public bool Success;

        public bool Delete;

        public string Message;

        public Texture2D Image;

        public Err Err;

        public List<InternalError> InternalErrors { get; set; }

        public T GetData<T>()
        {
            T result = JObject.Parse(Message)["data"].ToObject<T>();
            return result;
        }

        public Response()
        {
            Err = new Err();
            InternalErrors = new List<InternalError>();
        }
    }

    public class Err
    {
        public string message { get; set; }
        public List<string> errors { get; set; }

        public Err()
        {
            message = "";
            errors = new List<string>();
        }
    }

    public class InternalError
    {
        public InternalErrorHandler.ErrorTypes Code { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}