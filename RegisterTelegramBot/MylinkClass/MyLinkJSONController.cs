using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegBot2
{
    internal static class MyLinkJSONController
    {
        public static string AddLinkToJson(string JsonString, MyLink myLink)
        {
            List<MyLink> data;

            if (!string.IsNullOrEmpty(JsonString))
            {
                data = JsonConvert.DeserializeObject<List<MyLink>>(JsonString);
            }
            else
            {
                data = new List<MyLink>();
            }
            data.Add(myLink);

            var updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            return updatedJson;
        }

        public static string RemoveLinkFromJson(string constructorJson, MyLink myLink)
        {

            var data = JsonConvert.DeserializeObject<List<MyLink>>(constructorJson);

            data.Remove(myLink);

            var updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            return updatedJson;
        }
        public static List<MyLink> DecryptJsonToList(string json)
        {
            var decryptedList = JsonConvert.DeserializeObject<List<MyLink>>(json);
            return decryptedList;
        }
        public static string RemoveAll()
        {
            return "";

        }

    }
}
