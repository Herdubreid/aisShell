using System;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
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
        public (bool HasValue, int Parameter) Depth { get; set; }
        [Option("-fm|--formMembers", CommandOptionType.NoValue, Description = "Form Members")]
        public bool FormMembers { get; private set; }
        [Option("-gm|--gridMembers", CommandOptionType.NoValue, Description = "Grid Members")]
        public bool GridMembers { get; private set; }
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
                var res = Index.HasValue ? Responses[Index.Parameter] : Responses.Last();
                if (FormMembers) DisplayFormMembers(res);
                if (GridMembers) DisplayGridMembers(res);
                if (!FormMembers && !GridMembers) OutputLine((Key.HasValue ? res[Key.Parameter] : res.Result).ToString());
            }
            catch (Exception e)
            {
                Error("Response Error!\n{0}", e.Message);
            }
            return 1;
        }
    }
}
