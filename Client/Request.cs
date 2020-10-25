using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Request
    {
        /* Request class creates and handles requests
         to the Taki server. Each request has a 'code' field, and an 'args'
         field.
         */

        [JsonProperty(PropertyName = "code")]
        private string code;

        [JsonProperty(PropertyName = "args")]
        public Dictionary<string, dynamic> arguments; // value can be any type

        public Request(string code)
        {
            this.code = code;
            this.arguments = new Dictionary<string, dynamic>();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override string ToString()
        {
            string args = "";
            foreach (KeyValuePair<string, dynamic> kvp in this.arguments)
            {
                args += kvp.Key + " : " + kvp.Value + " ,";
            }
            return "{" + "'code' : '" + this.code + "', " + "'args' : " + "{" + args + "}";
        }
    }
}
