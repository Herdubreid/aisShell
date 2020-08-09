using System;
using McMaster.Extensions.CommandLineUtils;
using System.Text.Json;
namespace Celin
{
    public abstract class JObjectCmd : OutCmd
    {
        private JsonElement? _jToken;
        [Option("-k|--key", CommandOptionType.SingleValue, Description = "Object Key")]
        protected (bool HasValue, string Parameter) Key { get; }
        [Option("-d|--depth", CommandOptionType.SingleValue, Description = "Iteration Depth")]
        protected int? Depth { get; set; }
        protected void Trim(ref JsonElement jArray, ref int depth)
        {
            if (Depth.HasValue && Depth.Value < depth) return;
            foreach (var e in jArray.EnumerateArray())
            {
                switch (e.ValueKind)
                {
                    case JsonValueKind.Array:
                        var a = e;
                        Trim(ref a, ref depth);
                        break;
                    case JsonValueKind.Object:
                        var o = e;
                        Trim(ref o, ref depth);
                        break;
                }
            }
        }
        protected JsonElement? JToken
        {
            get => _jToken;
            set
            {
                if (Key.HasValue && value.Value.ValueKind == JsonValueKind.Object)
                {
                    _jToken = FindKey(value.Value);
                }
                else
                {
                    _jToken = value;
                }
                if (Depth.HasValue && _jToken != null)
                {
                    var depth = 0;
                    var o = JToken.Value;
                    Trim(ref o, ref depth);
                }
            }
        }
        protected Object Object
        {
            set
            {
                var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
                {
                    Converters =
                    {
                        new AIS.ActionJsonConverter(),
                        new AIS.GridActionJsonConverter()
                    }
                });
                _jToken = JsonSerializer.Deserialize<JsonElement>(json);
            }
        }
        protected JsonElement? FindKey(JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Array)
                foreach (var e in json.EnumerateArray())
                {
                    var res = FindKey(e);
                    if (res != null) return res;
                }
            else
            {
                if (json.TryGetProperty(Key.Parameter, out var res))
                    return res;
            }
            return null;
        }
        protected bool NullJToken
        {
            get
            {
                if (!JToken.HasValue)
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
            if (JToken.Value.ValueKind == JsonValueKind.Array) foreach (var e in JToken.Value.EnumerateArray()) OutputLine(e.ToString());
            else OutputLine(JsonSerializer.Serialize(JToken, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
        }
        protected bool Iter { get; set; } = false;
        protected override int OnExecute()
        {
            return base.OnExecute();
        }
    }
}
