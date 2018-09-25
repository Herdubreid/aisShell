using System;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json.Linq;
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
        protected JToken JToken { get; set; }
        protected override int OnExecute()
        {
            base.OnExecute();

            var ja = JToken.Type == JTokenType.Array ? JToken as JArray : null;
            if (ja is null)
            {
                Error("Not an array!");
                return 0;
            }
            try
            {
                var keys = Key.HasValue ? Key.Parameter.Split(';') : new string[] { "" };
                for (var i = From ?? 0; i < (To ?? ja.Count); i++)
                {
                    var e = ja[i];
                    if (e.Type == JTokenType.Object) foreach (var k in keys)
                        {
                            Output(e.SelectToken(k).ToString() + '\t');
                        }
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
