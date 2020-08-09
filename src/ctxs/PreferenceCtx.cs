namespace Celin
{
    public class PreferenceCtx : RequestCtx<AIS.PreferenceRequest, PreferenceCtx>
    {
        public PreferenceCtx(string id) : base("pr", id) { }
        public PreferenceCtx() : this(string.Empty) { }
    }
}
