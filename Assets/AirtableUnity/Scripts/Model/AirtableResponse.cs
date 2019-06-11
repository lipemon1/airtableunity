using System.Collections.Generic;

namespace AirtableUnity.PX.Model
{   
    public class AirtableResponse<T>
    {
        public string offset;
        public List<BaseRecord<T>> records = new List<BaseRecord<T>>();

        public AirtableResponse()
        {
            records = new List<BaseRecord<T>>();
        }
    }
}
