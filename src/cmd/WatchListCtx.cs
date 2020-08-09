using System;
using System.Collections.Generic;
using System.Text;

namespace Celin
{
    public class WatchListCtx : RequestCtx<AIS.WatchListRequest, WatchListCtx>
    {
        public WatchListCtx(string id) : base("wl", id) { }
        public WatchListCtx() : this(string.Empty) { }
    }
}
