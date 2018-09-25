namespace Celin
{
    public class StackFormCtx : RequestCtx<AIS.StackFormRequest, StackFormCtx>
    {
        public StackFormCtx(string id) : base("sfm", id)
        { }
    }
}
