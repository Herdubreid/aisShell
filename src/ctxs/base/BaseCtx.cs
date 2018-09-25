using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
namespace Celin
{
    class GridConverter : CustomCreationConverter<AIS.Grid>
    {
        public override AIS.Grid Create(Type objectType)
        {
            return null;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (jsonObject.ContainsKey("gridRowInsertEvents"))
            {
                return jsonObject.ToObject<AIS.GridInsert>();
            }
            return jsonObject.ToObject<AIS.GridUpdate>();
        }
    }
    class ActionConverter : CustomCreationConverter<AIS.Action>
    {
        public override AIS.Action Create(Type objectType)
        {
            return null;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            if (jsonObject.ContainsKey("controlID"))
            {
                return jsonObject.ToObject<AIS.FormAction>();
            }
            var ga = JsonConvert.DeserializeObject<AIS.GridAction>(jsonObject.ToString(), new GridConverter());
            return ga;
        }
    }
    public interface IBaseCtx
    {
        string Cmd { get; set; }
        string Id { get; set; }
    }
    public abstract class BaseCtx<T> : IBaseCtx
        where T : IBaseCtx
    {
        public static List<T> List { get; set; } = new List<T>();
        public static T Current { get; set; }
        public static bool Select(string id)
        {
            if (List.Exists(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                Current = List.Find(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                return true;
            }
            return false;
        }
        public static void Load(string fname)
        {
            try
            {
                using (StreamReader sr = File.OpenText(fname))
                {
                    var list = JsonConvert.DeserializeObject<List<T>>(sr.ReadToEnd(), new ActionConverter());
                    if (list != null && list.Count > 0)
                    {
                        List = list;
                        Current = List.First();
                    }
                    else BaseCmd.Warning("No data to load!");
                }
            }
            catch (Exception e)
            {
                BaseCmd.Error(e.Message);
            }
        }
        public static void Save(string fname)
        {
            if (!File.Exists(fname) || Prompt.GetYesNo("Do you want to overwrite existing file?", false))
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(fname))
                    {
                        sw.Write(JsonConvert.SerializeObject(List));
                    }
                }
                catch (Exception e)
                {
                    BaseCmd.Error(e.Message);
                }
            }
        }
        public string Id { get; set; }
        public string Cmd { get; set; }
        protected BaseCtx(string cmd, string id)
        {
            Cmd = cmd;
            Id = id;
        }
    }
}
