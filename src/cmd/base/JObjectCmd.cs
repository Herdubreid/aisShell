using System;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Celin
{
    public abstract class JObjectCmd : OutCmd
    {
        [Argument(0, Description = "Json Key")]
        protected (bool HasValue, string Parameter) Key { get; }
        protected JObject JObject { get; set; }
        protected Object Object
        {
            set
            {
                JObject = JObject.FromObject(value, new JsonSerializer()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
        }
        protected JToken FindKey(JArray json)
        {
            JToken res = null;
            foreach (var e in json)
            {
                switch (e.Type)
                {
                    case JTokenType.Array:
                        res = FindKey(e as JArray);
                        break;
                    case JTokenType.Object:
                        res = FindKey(e as JObject);
                        break;
                }
                if (res != null) return res;
            }
            return res;
        }
        protected JToken FindKey(JObject json)
        {
            if (!Key.HasValue) return json;
            var res = json.SelectToken(Key.Parameter);
            if (res is null)
            {
                foreach (var e in json)
                {
                    switch (e.Value.Type)
                    {
                        case JTokenType.Array:
                            res = FindKey(e.Value as JArray);
                            break;
                        case JTokenType.Object:
                            res = FindKey(e.Value as JObject);
                            break;
                    }
                    if (res != null) return res;
                }
            }
            return res;
        }
        protected void Dump()
        {
            if (Key.HasValue)
            {
                var token = FindKey(JObject);
                if (token is null) Warning("Key '{0}' not found!", Key.Parameter);
                else if (token.Type == JTokenType.Array) foreach (var e in token as JArray) OutputLine(e.ToString());
                else OutputLine(token.ToString());
            }
            else OutputLine(JObject.ToString());
        }
        protected override int OnExecute()
        {
            base.OnExecute();

            return 1;
        }
    }
}
