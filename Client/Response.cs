using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Client
{
    class Response
    {
        /* Response class creates and handles responses
         from the Taki server. Each response has a 'code'/'status'
         field and an 'args' field.
         */

        [JsonProperty(PropertyName = "code")]
        public string code;

        [JsonProperty(PropertyName = "status")]
        private string status { set {code = value; } } // Set code with the value of the status

        [JsonProperty(PropertyName = "args")]
        public Dictionary<string, dynamic> arguments;

        public Response()
        {

        }

        public override string ToString()
        {
            string args = "";
            foreach (KeyValuePair<string, dynamic> kvp in this.arguments)
            {
                args += kvp.Key + " : " + kvp.Value + " ,";
            }
            return "{" + "'code' : '"+ this.code +"', " + "'args' : " + "{" + args + "}";
        }
    }
}
