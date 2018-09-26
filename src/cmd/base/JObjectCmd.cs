using System;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Celin
{
    public abstract class JObjectCmd : OutCmd
    {
        private JToken _jToken;
        [Option("-k|--key", CommandOptionType.SingleValue, Description = "Object Key")]
        protected (bool HasValue, string Parameter) Key { get; }
        [Option("-d|--depth", CommandOptionType.SingleValue, Description = "Iteration Depth")]
        protected int? Depth { get; set; }
        protected void Trim(ref JArray jArray, ref int depth)
        {
            if (Depth.HasValue && Depth.Value < depth)jArray.RemoveAll();
            foreach (var e in jArray)
            {
                switch (e.Type)
                {
                    case JTokenType.Array:
                        var a = e as JArray;
                        Trim(ref a, ref depth);
                        break;
                    case JTokenType.Object:
                        var o = e as JObject;
                        Trim(ref o, ref depth);
                        break;
                }
            }
        }
        protected void Trim(ref JObject jObject, ref int depth)
        {
            depth++;
            foreach (var e in jObject)
            {
                switch (e.Value.Type)
                {
                    case JTokenType.Array:
                        var a = e.Value as JArray;
                        Trim(ref a, ref depth);
                        break;
                    case JTokenType.Object:
                        var o = e.Value as JObject;
                        if (Depth.HasValue && Depth.Value < depth) o.RemoveAll();
                        else Trim(ref o, ref depth);
                        break;
                }
            }
            depth--;
        }
        protected JToken JToken
        {
            get => _jToken;
            set
            {
                if (Key.HasValue && value.Type == JTokenType.Object)
                {
                    _jToken = FindKey(value as JObject);
                }
                else
                {
                    _jToken = value;
                }
                if (Depth.HasValue && _jToken != null)
                {
                    var depth = 0;
                    switch (JToken.Type)
                    {
                        case JTokenType.Object:
                            var o = JToken as JObject;
                            Trim(ref o, ref depth);
                            break;
                        case JTokenType.Array:
                            var a = JToken as JArray;
                            Trim(ref a, ref depth);
                            break;
                    }
                }
            }
        }
        protected Object Object
        {
            set
            {
                JToken = JToken.FromObject(value, new JsonSerializer()
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
        protected bool NullJToken
        {
            get
            {
                if (JToken is null)
                {
                    Warning("Key '{0}' not found!", Key.Parameter);
                    return true;
                }
                return false;
            }
        }

        protected void Dump()
        {
            if (NullJToken) return;
            if (JToken.Type == JTokenType.Array) foreach (var e in JToken as JArray) OutputLine(e.ToString());
            else OutputLine(JToken.ToString());
        }
        protected bool Iter { get; set; } = false;
        protected override int OnExecute()
        {
            return base.OnExecute();
        }
    }
}
