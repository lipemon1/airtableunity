using System.Collections.Generic;

namespace AirtableUnity.PX.Model
{
    public class BaseRecord<T>
    {
        public string id;
        public T fields;
        public string createdTime;
    }
}