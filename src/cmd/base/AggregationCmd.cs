using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
namespace Celin
{
    public abstract class AggregationCmd : BaseCmd
    {
        [Option("-rm|--remove", CommandOptionType.NoValue, Description = "Remove Aggregation")]
        protected bool Remove { get; }
        [Option("-a|--aggregation", CommandOptionType.SingleValue, Description = "Aggregation")]
        [AllowedValues(new string []
        {
            "SUM",
            "MIN",
            "MAX",
            "AVG",
            "COUNT",
            "COUNT_DISTINCT",
            "AVG_DISTINCT"
        }, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) Aggregation { get; }
        [Option("-o|--order", CommandOptionType.SingleValue, Description = "Direction")]
        [AllowedValues(new string [] { "ASC", "DESC"}, IgnoreCase = true)]
        protected (bool HasValue, string Parameter) Direction { get; }
        [Argument(0, "Column")]
        [Required]
        protected (bool HasValue, string Parameter) Column { get; }
        protected List<AIS.AggregationItem> Aggregations { get; set; }
        protected virtual int OnExecute()
        {
            if (!Remove)
            {
                Aggregations.Add(new AIS.AggregationItem()
                {
                    column = Column.Parameter.ToUpper(),
                    aggregation = Aggregation.HasValue ? Aggregation.Parameter.ToUpper() : null,
                    direction = Direction.HasValue ? Direction.Parameter.ToUpper() : null
                });
            }
            else
            {
                var ag = Aggregations.Find(e => e.column.Equals(Column.Parameter, StringComparison.OrdinalIgnoreCase));
                if (ag is null) Error("Column {0} not found!", Column.Parameter);
                else Aggregations.Remove(ag);
            }

            return 1;
        }
    }
}
