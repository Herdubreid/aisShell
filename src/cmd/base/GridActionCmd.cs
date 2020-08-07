using System;
using System.Linq;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public class GridActionCmd<T> : BaseCmd
        where T : AIS.Grid, new()
    {
        [Option("-g|--gridId", CommandOptionType.SingleValue, Description = "Grid Id")]
        protected (bool HasValue, string Parameter) Id { get; set; }
        [Option("-r|--rowNumber", CommandOptionType.SingleValue, Description = "Row Number")]
        protected (bool HasValue, int Parameter) RowNumber { get; set; }
        [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Column Event")]
        protected bool Remove { get; }
        [Argument(0, Description = "Control Id")]
        protected (bool HasValue, string Parameter) ColumnID { get; set; }
        [Argument(1, Description = "Command")]
        [AllowedValues(new string[]
        {
                "SetGridCellValue",
                "SetGridComboValue"
        })]
        protected (bool HasValue, string Parameter) Command { get; set; }
        [Argument(2, Description = "Value")]
        protected (bool HasValue, string Parameter) Value { get; set; }
        public virtual List<AIS.RowEvent> RowEvents(T action)
        {
            return null;
        }
        public List<AIS.Action> FormActions { get; set; }
        protected List<string> RemainingArguments { get; }
        protected virtual int OnExecute()
        {
            var action = FormActions.Find(a =>
            {
                if (a.GetType() == typeof(AIS.GridAction))
                {
                    var ga = a as AIS.GridAction;
                    if (ga.gridAction.GetType() == typeof(T))
                    {
                        return !Id.HasValue || ga.gridAction.gridID.Equals(Id.Parameter, StringComparison.OrdinalIgnoreCase);
                    }
                }
                return false;
            }) as AIS.GridAction;

            if (action is null)
            {
                if (!Id.HasValue)
                {
                    Error("Grid Id required!");
                    return 0;
                }

                action = new AIS.GridAction()
                {
                    gridAction = new T()
                };
                action.gridAction.gridID = Id.Parameter;
                FormActions.Add(action);
            }

            var gridAction = action.gridAction as T;
            var rowEvents = RowEvents(gridAction);

            var rowEvent = RowNumber.HasValue
                    ? rowEvents.Find(e => e.rowNumber == RowNumber.Parameter)
                    : rowEvents.Count > 0 ? rowEvents.Last() : null;

            if (rowEvent is null)
            {
                if (!RowNumber.HasValue)
                {
                    Error("Row Number required!");
                    return 0;
                }
                rowEvent = new AIS.RowEvent()
                {
                    rowNumber = RowNumber.Parameter,
                    gridColumnEvents = new List<AIS.ColumnEvent>()
                };
                rowEvents.Add(rowEvent);
            }
            var ce = ColumnID.HasValue ? rowEvent.gridColumnEvents.Find(e => e.columnID.Equals(ColumnID.Parameter, StringComparison.OrdinalIgnoreCase)) : null;
            if (ColumnID.HasValue && Command.HasValue)
            {
                var value = Value.HasValue ? RemainingArguments.Count > 0
                                 ? Value.Parameter + " " + RemainingArguments.Aggregate((a, s) => a + " " + s)
                                 : Value.Parameter : "";
                if (ce is null) rowEvent.gridColumnEvents.Add(new AIS.ColumnEvent()
                {
                    columnID = ColumnID.Parameter,
                    command = Command.Parameter,
                    value = value
                });
                else
                {
                    ce.command = Command.Parameter;
                    ce.value = value;
                }
            }
            else if (ColumnID.HasValue)
            {
                if (ce is null) Error("Column Id {0} not found!", ColumnID.Parameter);
                else if (Remove) rowEvent.gridColumnEvents.Remove(ce);
            }

            return 1;
        }
        public GridActionCmd(T grid)
        {
            Id = (false, grid.gridID);
        }
        public GridActionCmd() { }
    }
}
