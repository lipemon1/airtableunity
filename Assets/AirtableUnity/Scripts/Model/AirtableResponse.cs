using System.Collections.Generic;

namespace AirtableUnity.PX.Model
{   
    public class AirtableResponse<T>
    {
        public string offset;
        public List<Record<T>> records = new List<Record<T>>();

        public AirtableResponse()
        {
            records = new List<Record<T>>();
        }
    }
}
