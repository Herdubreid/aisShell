using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Celin
{
    public interface IBaseCtx
    {
        string Cmd { get; set; }
        string Id { get; set; }
    }
    public abstract class BaseCtx<T> : IBaseCtx
        where T : IBaseCtx
    {
        public string Id { get; set; }
        public string Cmd { get; set; }
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
                    var list = JsonSerializer.Deserialize<List<T>>(sr.ReadToEnd(), new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new ServerJsonConverter(),
                            new AIS.ActionJsonConverter(),
                            new AIS.GridActionJsonConverter()
                        }
                    });
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
                        sw.Write(JsonSerializer.Serialize(List));
                    }
                }
                catch (Exception e)
                {
                    BaseCmd.Error(e.Message);
                }
            }
        }
        public static bool Wait<T1>(T1 task, CancellationTokenSource cancel = null)
            where T1 : Task
        {
            var wm = @"|/-\";
            var i = 0;
            Console.CursorVisible = false;
            while (!task.IsCompleted)
            {
                Thread.Sleep(400);
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    Console.CursorVisible = true;
                    if (Prompt.GetYesNo("\rCancel Request?", false))
                    {
                        cancel.Cancel();
                        BaseCmd.Error("Request Cancelled!");
                        break;
                    }
                    Console.CursorVisible = false;
                }
                Console.Write('\r' + wm[i].ToString());
                i = (i + 1) % 4;
            }
            Console.CursorVisible = true;
            return task.IsCompleted;
        }
        protected BaseCtx(string cmd, string id)
        {
            Cmd = cmd;
            Id = id;
        }
    }
}
