using System.Collections.Generic;

namespace AirtableUnity.PX.Model
{   
    public class AirtableResponse<T>
    {
        public string offset;
        public List<T> records = new List<T>();

        public AirtableResponse()
        {
            records = new List<T>();
        }
    }
}
