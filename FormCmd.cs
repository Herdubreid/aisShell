using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Celin
{
    public class Response
    {
        public FormCtx Request { get; set; }
        public JObject Result { get; set; }
    }
    [Command("fm", Description = "AIS Forms")]
    [Subcommand("d", typeof(DefCmd))]
    [Subcommand("s", typeof(SubCmd))]
    [Subcommand("r", typeof(ResCmd))]
    [Subcommand("save", typeof(SaveCmd))]
    [Subcommand("load", typeof(LoadCmd))]
    public class FormCmd : BaseCmd
    {
        [Option("-id", CommandOptionType.SingleValue, Description = "Form Context Id")]
        public (bool HasValue, string Parameter) Id { get; }
        static string LastFileName { get; set; }
        [Command(Description = "Load Form")]
        class LoadCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                var fname = FileName.Parameter + ".fctx";
                try
                {
                    using (StreamReader sr = File.OpenText(fname))
                    {
                        var data = (JArray)JsonConvert.DeserializeObject(sr.ReadToEnd());
                        if (data.Count > 0)
                        {
                            FormCtx.List = data.ToObject<List<FormCtx>>();
                            FormCtx.Current = FormCtx.List.First();
                            LastFileName = FileName.Parameter;
                        }
                        else Warning("No data to load!");
                    }
                }
                catch (Exception e)
                {
                    Error(e.Message);
                }
                return 1;
            }
        }
        [Command(Description = "Save Forms")]
        class SaveCmd : BaseCmd
        {
            [Argument(0, Description = "File Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FileName { get; private set; }
            int OnExecute()
            {
                PromptOptions();
                var fname = FileName.Parameter + ".fctx";
                if (!File.Exists(fname) || Prompt.GetYesNo("Do you want to overwrite existing file?", false))
                {
                    try
                    {
                        using (StreamWriter sw = File.CreateText(fname))
                        {
                            sw.Write(JsonConvert.SerializeObject(FormCtx.List));
                        }
                        LastFileName = FileName.Parameter;
                    }
                    catch (Exception e)
                    {
                        Error(e.Message);
                    }
                }
                return 1;
            }
            public SaveCmd()
            {
                FileName = (false, LastFileName);
            }
        }
        [Command(Description = "Response")]
        class ResCmd : BaseCmd
        {
            public static List<Response> List { get; } = new List<Response>();
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
            void DisplayGridMembers(Response res)
            {
                var grid = (JArray)res.Result.SelectToken(String.Format("fs_{0}.data.gridData.rowset", res.Request.Request.formName));
                if (grid is null) Error("No Grid Member!");
                else if (grid.Count == 0) Warning("Grid is Empty!");
                else foreach (var e in (JObject)grid[0]) OutputLine(String.Format("{0, -30}{1}", e.Key, e.Value.HasValues ? e.Value["value"] : e.Value));
            }
            void DisplayFormMembers(Response res)
            {
                var fm = (JObject)res.Result.SelectToken(String.Format("fs_{0}.data", res.Request.Request.formName));
                if (fm is null) Error("No Form Member!");
                else foreach (var e in fm) OutputLine(String.Format("{0, -30}{1}", e.Key, e.Value["value"]));
            }
            JToken FindKey(JArray json, string key)
            {
                foreach (var e in json)
                {
                    var res = e.Children().Any() ?
                                e.GetType() == typeof(JArray) ?
                                FindKey((JArray)e, key) :
                                FindKey((JObject)e, key) : null;
                    if (res != null) return res;
                }
                return null;
            }
            JToken FindKey(JObject json, string key)
            {
                var res = json[key];
                if (res is null)
                {
                    foreach (var e in json)
                    {
                        res = e.Value.Children().Any() ?
                                    e.Value.GetType() == typeof(JArray) ?
                                    FindKey((JArray)e.Value, key) :
                                    FindKey((JObject)e.Value, key) : null;
                        if (res != null) return res;
                    }
                }
                return res;
            }
            int OnExecute()
            {
                try
                {
                    var res = Index.HasValue ? List[Index.Parameter] : List.Last();
                    if (FormMembers) DisplayFormMembers(res);
                    if (GridMembers) DisplayGridMembers(res);
                    if (!FormMembers && !GridMembers) OutputLine((Key.HasValue ? FindKey(res.Result, Key.Parameter) : res.Result).ToString());
                }
                catch (Exception e)
                {
                    Error("Invalid Response Context!\n{0}", e.Message);
                }
                return 1;
            }
        }
        [Command(Description = "Define")]
        [Subcommand("a", typeof(ActCmd))]
        public class DefCmd : BaseCmd
        {
            [Command(Description = "Form Action")]
            class ActCmd : BaseCmd
            {
                [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Form Action")]
                [SuppressDisplay]
                public bool Remove { get; private set; }
                [Argument(0, Description = "Control Id")]
                public (bool HasValue, string Parameter) ControlID { get; private set; }
                [Argument(1, Description = "Command")]
                [AllowedValues(new string[]
                {
                "SetControlValue",
                "SetQBEValue",
                "DoAction",
                "SetRadioButton",
                "SetComboValue",
                "SetCheckboxValue",
                "SelectRow",
                "UnSelectRow",
                "UnSelectAllRows",
                "SelectAllRows",
                "ClickGridCell",
                "ClickGridColumnAggregate"
                })]
                public (bool HasValue, string Parameter) Command { get; private set; }
                [Argument(2, Description = "Value")]
                public (bool HasValue, string Parameter) Value { get; private set; }
                int OnExecute()
                {
                    if (FormCtx.Current is null)
                    {
                        Error("No Current Form Context!");
                    }
                    else
                    {
                        var fas = FormCtx.Current.Request.formActions;
                        if (ControlID.HasValue && Command.HasValue)
                        {
                            fas.Add(new AIS.FormAction()
                            {
                                controlID = ControlID.Parameter,
                                command = Command.Parameter,
                                value = Value.Parameter
                            });
                        }
                        else if (ControlID.HasValue)
                        {
                            var fa = fas.Single(e => e.controlID.Equals(ControlID.Parameter));
                            if (fa is null) Warning("ControlID {0} not found!", ControlID.Parameter);
                            else if (Remove) fas.Remove(fa);
                            else
                            {
                                var cmd = new ActCmd(fa);
                                cmd.Display(OutFile, true);
                            }
                        }
                        else
                        {
                            foreach (var fa in fas)
                            {
                                var cmd = new ActCmd(fa);
                                cmd.Display(OutFile, true);
                            }
                        }
                    }
                    return 1;
                }
                public ActCmd(AIS.FormAction fa)
                {
                    ControlID = (false, fa.controlID);
                    Command = (false, fa.command);
                    Value = (false, fa.value);
                }
                public ActCmd()
                { }
            }
            [Option("-fn|--formName", CommandOptionType.SingleValue, Description = "Form Name")]
            [PromptOption]
            public (bool HasValue, string Parameter) FormName { get; private set; }
            [Option("-v|--version", CommandOptionType.SingleValue, Description = "Version Name")]
            public (bool HasValue, string Parameter) Version { get; private set; }
            [Option("-f|--find", CommandOptionType.SingleValue, Description = "Find on Entry")]
            [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
            public (bool HasValue, string Parameter) FindOnEntry { get; private set; }
            [Option("-ri|--returnIds", CommandOptionType.SingleValue, Description = "Return Control IDs")]
            public (bool HasValue, string Parameter) ReturnControlIDs { get; private set; }
            [Option("-mp|--maxPage", CommandOptionType.SingleValue, Description = "Max Page Size")]
            public (bool HasValue, int Parameter) MaxPageSize { get; private set; }
            [Option("-an|--aliasNaming", CommandOptionType.SingleValue, Description = "Alias Naming")]
            [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
            public (bool HasValue, string Parameter) AliasNaming { get; private set; }
            [Option("-ot|--outputType", CommandOptionType.SingleValue, Description = "Output Type")]
            public (bool HasValue, string Parameter) OutputType { get; private set; }
            [Option("-fa|--formAction", CommandOptionType.SingleValue, Description = "Form Service Action")]
            public (bool HasValue, string Parameter) FormServiceAction { get; private set; }
            [Option("-sw|--stopOnWarning", CommandOptionType.SingleValue, Description = "Stop on Warning")]
            [AllowedValues(new string[] { "true", "false" }, IgnoreCase = true)]
            public (bool HasValue, string Parameter) StopOnWarning { get; private set; }
            [Option("-qn|--queryName", CommandOptionType.SingleValue, Description = "Query Object Name")]
            public (bool HasValue, string Parameter) QueryObjectName { get; private set; }
            FormCmd FormCmd { get; set; }
            int OnExecute()
            {
                if (FormCmd.OnExecute() == 0 && FormCmd.Id.HasValue)
                {
                    if (Prompt.GetYesNo("New Form Definition?", true))
                    {
                        PromptOptions();
                        FormCtx.Current = new FormCtx(FormCmd.Id.Parameter);
                        FormCtx.List.Add(FormCtx.Current);
                    }
                }
                if (FormCtx.Current is null)
                {
                    Error("No Form Context!");
                    return 1;
                }
                var rq = FormCtx.Current.Request;
                rq.formName = FormName.HasValue ? FormName.Parameter : rq.formName;
                rq.version = Version.HasValue ? Version.Parameter : rq.version;
                rq.findOnEntry = FindOnEntry.HasValue ? FindOnEntry.Parameter.ToUpper() : rq.findOnEntry;
                rq.returnControlIDs = ReturnControlIDs.HasValue ? ReturnControlIDs.Parameter : rq.returnControlIDs;
                rq.maxPageSize = MaxPageSize.HasValue ? MaxPageSize.Parameter.ToString() : rq.maxPageSize;
                rq.aliasNaming = AliasNaming.HasValue ? AliasNaming.Parameter.Equals("true", StringComparison.OrdinalIgnoreCase) : rq.aliasNaming;
                rq.outputType = OutputType.HasValue ? OutputType.Parameter : rq.outputType;
                rq.formServiceAction = FormServiceAction.HasValue ? FormServiceAction.Parameter : rq.formServiceAction;
                rq.stopOnWarning = StopOnWarning.HasValue ? StopOnWarning.Parameter.ToUpper() : rq.stopOnWarning;
                rq.queryObjectName = QueryObjectName.HasValue ? QueryObjectName.Parameter : rq.queryObjectName;

                var cmd = new DefCmd(FormCtx.Current);
                cmd.Display(OutFile, false);
                OutputLine(OutFile);

                return 1;
            }
            public DefCmd(FormCtx formCtx)
            {
                var rq = formCtx.Request;
                FormName = (false, rq.formName);
                Version = (false, rq.version);
                FindOnEntry = (false, rq.findOnEntry);
                ReturnControlIDs = (false, rq.returnControlIDs);
                MaxPageSize = (false, Convert.ToInt32(rq.maxPageSize));
                AliasNaming = (false, rq.aliasNaming ? "TRUE" : "FALSE");
                OutputType = (false, rq.outputType);
                FormServiceAction = (false, rq.formServiceAction);
                StopOnWarning = (false, rq.stopOnWarning);
                QueryObjectName = (false, rq.queryObjectName);
            }
            public DefCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        [Command(Description = "Submit")]
        public class SubCmd : BaseCmd
        {
            FormCmd FormCmd { get; set; }
            int OnExecute()
            {
                FormCmd.OnExecute();
                if (ServerCtx.Current is null)
                {
                    Error("No Server Context!");
                }
                else if (ServerCtx.Current.Server.AuthResponse is null)
                {
                    Error("{0} not connected!", ServerCtx.Current.Id);
                }
                else
                {
                    if (FormCtx.Current is null)
                    {
                        Error("No Form Context!");
                    }
                    else
                    {
                        var t = new Task<Tuple<bool, JObject>>(() => ServerCtx.Current.Server.Request<JObject>(FormCtx.Current.Request));
                        t.Start();
                        while (!t.IsCompleted)
                        {
                            Thread.Sleep(500);
                            Console.Write('.');
                        }
                        if (t.Result.Item1)
                        {
                            ResCmd.List.Add(new Response() { Request = FormCtx.Current, Result = t.Result.Item2 });
                            Success("Responses {0}.", ResCmd.List.Count);
                        }
                        else
                        {
                            Error("Form submit failed!");
                        }
                    }
                }
                return 1;
            }
            public SubCmd(FormCmd formCmd)
            {
                FormCmd = formCmd;
            }
        }
        public int OnExecute()
        {
            if (Id.HasValue)
            {
                return FormCtx.Select(Id.Parameter) ? 1 : 0;
            }
            /*foreach (var ctx in FormCtx.List)
            {
                var cmd = new DefCmd(ctx);
                cmd.Display(OutFile, false);
                OutputLine(OutFile);
            }*/
            return 1;
        }
        public static void AddCmd()
        {
            Commands.Add(new Cmd()
            {
                Type = typeof(FormCmd),
                Execute = CommandLineApplication.Execute<FormCmd>
            });
        }
        public static void RemoveCmd()
        {
            Commands.Remove(Commands.Single<Cmd>(c => c.Type == typeof(FormCmd)));
        }
    }
}