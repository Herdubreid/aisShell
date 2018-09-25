namespace Celin
{
    public class ServerCtx : BaseCtx<ServerCtx>
    {
        public AIS.Server Server { get; set; }
        public ServerCtx(string id, string baseUrl) : base("sv", id)
        {
            Server = new AIS.Server(baseUrl);
        }
    }
}
