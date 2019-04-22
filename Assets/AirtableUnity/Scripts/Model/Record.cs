using System.Collections.Generic;

namespace AirtableUnity.PX.Model
{
    public class Record
    {
        public string id;
        public List<Field> fields = new List<Field>();
        public string createdTime;
    }
}