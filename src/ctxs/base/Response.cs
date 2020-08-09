using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
namespace Celin
{
    public class Response<T> where T : AIS.Service
    {
        public T Request { get; set; }
        public JsonElement Result { get; set; }
        public string ResultKey
        {
            get
            {
                if (typeof(T) == typeof(AIS.FormRequest)) return string.Format("fs_{0}", Request.formName);
                if (typeof(T) == typeof(AIS.DatabrowserRequest))
                {
                    var rq = Request as AIS.DatabrowserRequest;
                    return string.Format("fs_DATABROWSE_{0}", rq.targetName);
                }
                if (typeof(T) == typeof(AIS.StackFormRequest))
                {
                    var rq = Request as AIS.StackFormRequest;
                    return string.Format("fs_{0}", rq.formRequest.formName);
                }

                return null;
            }
        }
        public List<ValueTuple<string, JsonElement>> GridMembers
        {
            get
            {
                var result = new List<ValueTuple<string, JsonElement>>();
                if (Result.TryGetProperty(ResultKey, out var grid))
                    if (grid.TryGetProperty("data", out grid))
                        if (grid.TryGetProperty("gridData", out grid))
                            if (grid.TryGetProperty("rowset", out grid))
                            {
                                if (grid.EnumerateArray().Count() == 0) BaseCmd.Warning("Grid is Empty!");
                                else foreach (var e in grid[0].EnumerateObject()) result.Add((e.Name, e.Value));
                            }
                else BaseCmd.Error("No Grid Member!");
                return result;
            }
        }
    }
}
