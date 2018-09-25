﻿using System;
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
        protected bool NullJToken()
        {
            if (JToken is null)
            {
                Warning("Key '{0}' not found!", Key.Parameter);
                return true;
            }
            return false;
        }
        protected void Dump()
        {
            if (NullJToken()) return;
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