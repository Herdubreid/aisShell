﻿namespace Celin
{
    public class DataCtx : RequestCtx<AIS.DatabrowserRequest, DataCtx>
    {
        public DataCtx(string id) : base("dt", id)
        {
            Request.findOnEntry = "TRUE";
        }
    }
}
