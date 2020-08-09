using System;
using McMaster.Extensions.CommandLineUtils;
using System.Text.Json;
namespace Celin
{
    public class JArrayCmd : OutCmd
    {
        [Argument(0, Description = "Key (separate multiple keys with ';'")]
        protected (bool HasValue, string Parameter) Key { get; }
        [Argument(1, Description = "From")]
        protected int? From { get; }
        [Argument(2, Description = "To")]
        protected int? To { get; }
        protected JsonElement JToken { get; set; }
        protected override int OnExecute()
        {
            base.OnExecute();

            if (JToken.ValueKind != JsonValueKind.Array)
            {
                Error("Not an array!");
                return 0;
            }
            try
            {
                var keys = Key.HasValue ? Key.Parameter.Split(';') : new string[] { "" };
                foreach (var e in JToken.EnumerateArray())
                {
                    if (e.ValueKind == JsonValueKind.Object)
                        foreach (var k in e.EnumerateObject()) Output(k.ToString() + '\t');
                    else Output(e.ToString() + '\t');
                    OutputLine();
                }
            }
            catch (Exception e)
            {
                Error("Key or index invalid!\n" + e.Message);
            }

            return 1;
        }
    }
}
