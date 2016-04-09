using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Attendance.Common.Util
{
    public class ObjToJson
    {
        public static String ToJson(Object obj)
        {
            string json = "";
            json = JsonConvert.SerializeObject(obj);
            return json;
        }
        public static Object JsonToModel(string jsonstring, Object model) {

            model = JsonConvert.DeserializeObject<Object>(jsonstring);
            return model;
        }
    }
}
