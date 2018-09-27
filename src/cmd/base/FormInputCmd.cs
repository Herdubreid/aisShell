using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public class FormInputCmd : BaseCmd
    {
        [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Form Input")]
        protected bool Remove { get; }
        [Argument(0, Description = "Id")]
        protected (bool HasValue, string Parameter) Id { get; set; }
        [Argument(1, Description = "Value")]
        protected (bool HasValue, string Parameter) Value { get; set; }
        protected List<AIS.Input> FormInputs { get; set; }
        protected List<string> RemainingArguments { get; }
        protected virtual int OnExecute()
        {
            var ia = Id.HasValue ? FormInputs.Find(e => e.id.Equals(Id.Parameter)) : null;
            if (Id.HasValue && Value.HasValue)
            {
                var value = Value.HasValue ? RemainingArguments.Count > 0
                                 ? Value.Parameter + " " + RemainingArguments.Aggregate((a, s) => a + " " + s)
                                 : Value.Parameter : "";
                if (ia is null) FormInputs.Add(new AIS.Input()
                {
                    id = Id.Parameter,
                    value = value
                });
                else ia.value = value;
            }
            else if (Id.HasValue)
            {
                if (ia is null) Error("Id {0} not found!", Id.Parameter);
                else if (Remove) FormInputs.Remove(ia);
            }

            return 1;
        }
        public FormInputCmd(AIS.Input i)
        {
            Id = (false, i.id);
            Value = (false, i.value);
        }
        public FormInputCmd()
        {
        }
    }
}
