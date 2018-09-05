using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
namespace Celin
{
    public class CtxId
    {
        public string Id { get; set; }
        public CtxId(string id)
        {
            Id = id;
        }
        public CtxId()
        {}
    }
    public class FormCtx : CtxId
    {
        public static List<FormCtx> List { get; set; } = new List<FormCtx>();
        public static FormCtx Current { get; set; }
        public static FormCtx Select(string id)
        {
            Current = List.Find(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Current;
        }
        public AIS.FormRequest Request { get; set; } = new AIS.FormRequest();
        public FormCtx(string id) : base(id)
        {
            Request.maxPageSize = "10";
            Request.formServiceAction = "R";
        }
    }
    public class ServerCtx : CtxId
    {
        public static List<ServerCtx> List { get; set; } = new List<ServerCtx>();
        public static ServerCtx Current { get; set; }
        public static ServerCtx Select(string id)
        {
            Current = List.Find(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Current;
        }
        public AIS.Server Server { get; set; }
        public ServerCtx(string id, string baseUrl) : base(id)
        {
            Server = new AIS.Server(baseUrl);
        }
    }
}
