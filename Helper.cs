using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NachaFileParser
{
    public class Helper
    {
        public static JObject SerializeRecord(IWriteableRecord record)
        {
            string jsonString = JsonConvert.SerializeObject(record);
            return JObject.Parse(jsonString);
        }
    }
}