using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Celin
{
    public class ServerJsonConverter : JsonConverter<AIS.Server>
    {
        public override AIS.Server Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            return new AIS.Server(json.GetProperty("BaseUrl").GetString())
            {
                AuthRequest = JsonSerializer.Deserialize<AIS.AuthRequest>(json.GetProperty("AuthRequest").ToString())
            };
        }

        public override void Write(Utf8JsonWriter writer, AIS.Server value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
