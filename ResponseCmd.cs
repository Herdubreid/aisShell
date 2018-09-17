using System;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Celin
{
    public abstract class ResponseCmd<T> : OutCmd where T : AIS.Request
    {
        [Option("-i|--index", CommandOptionType.SingleValue, Description = "Response's Zero Based Index")]
        public (bool HasValue, int Parameter) Index { get; private set; }
        [Option("-k|--key", CommandOptionType.SingleValue, Description = "Response Key Name")]
        public (bool HasValue, string Parameter) Key { get; private set; }
        [Option("-d|--depth", CommandOptionType.SingleValue, Description = "Iteration Depth")]
        public int? Depth { get; set; }
        [Option("-fm|--formMembers", CommandOptionType.NoValue, Description = "Form Members")]
        public bool FormMembers { get; private set; }
        [Option("-gm|--gridMembers", CommandOptionType.NoValue, Description = "Grid Members")]
        public bool GridMembers { get; private set; }
        public bool OutputChildren(JArray jArray, ref int depth)
        {
            if (Depth.HasValue && depth > Depth.Value) return false;
            OutputLine("[");
            foreach (var t in jArray)
            {
                if (t.Type == JTokenType.Object)
                {
                    Output(String.Format("{0," + depth + "}", String.Empty));
                    OutputChildren(t as JObject, ref depth);
                }
                if (t.Type == JTokenType.Array)
                {
                    OutputChildren(t as JArray, ref depth);
                }
            }
            OutputLine(String.Format("{0," + depth + "}],", String.Empty));
            return true;
        }
        public bool OutputChildren(JObject jObject, ref int depth)
        {
            if (Depth.HasValue && depth > Depth.Value) return false;
            OutputLine("{");
            depth++;
            foreach (var t in jObject)
            {
                OutputValue(t, ref depth);
            }
            depth--;
            OutputLine(String.Format("{0," + depth + "}{1},", String.Empty, "}"));
            return true;
        }
        public void OutputValue(KeyValuePair<string, JToken> pair, ref int depth)
        {
            var fmt = "{0," + depth + "}";
            Output(String.Format(fmt + "{1}: ", String.Empty, pair.Key));
            if (pair.Value.Type == JTokenType.Object)
            {
                if (!OutputChildren(pair.Value as JObject, ref depth))
                {
                    OutputLine("{...},");
                }
            }
            else if (pair.Value.Type == JTokenType.Array)
            {
                if (!OutputChildren(pair.Value as JArray, ref depth))
                {
                    OutputLine("[...]");
                }
            }
            else OutputLine(String.Format(pair.Value.Type == JTokenType.String ? "'{0}'," : "{0},", pair.Value));
        }
        void DisplayGridMembers(Response<T> response)
        {
            foreach (var e in response.GridMembers) OutputLine(String.Format("{0, -30}{1}", e.Item1, e.Item2.HasValues ? e.Item2["value"] : e.Item2));
        }
        void DisplayFormMembers(Response<T> response)
        {
            var fm = (JObject)response.Result.SelectToken(String.Format("fs_{0}.data", response.Request.formName));
            if (fm is null) Error("No Form Member!");
            else foreach (var e in fm) OutputLine(String.Format("{0, -30}{1}", e.Key, e.Value["value"]));
        }
        protected List<Response<T>> Responses { get; set; }
        protected int OnExecute()
        {
            try
            {
                var depth = 0;
                var res = Index.HasValue ? Responses[Index.Parameter] : Responses.Last();
                if (FormMembers) DisplayFormMembers(res);
                if (GridMembers) DisplayGridMembers(res);
                if (!FormMembers && !GridMembers)
                {
                    var j = Key.HasValue ? res[Key.Parameter] : res.Result;
                    if (j.Type == JTokenType.Object) OutputChildren(j as JObject, ref depth);
                    if (j.Type == JTokenType.Array) OutputChildren(j as JArray, ref depth);
                }
            }
            catch (Exception e)
            {
                Error("Response Error!\n{0}", e.Message);
            }
            return 1;
        }
    }
}
