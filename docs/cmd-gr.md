# Group By [Usage](../README.md#commands)
### Subcommand [`gr`](./cmd-dt.md)
```
Usage: dt gr [arguments] [options]

Arguments:
  Column            

Options:
  -rm|--remove      Remove Aggregatin
  -a|--aggregation  Aggregation
  -o|--order        Direction
  -?|-h|--help      Show help information
```
**Note:** The `Server Context` requires the `dataServiceAggregation` capability.

## Arguments
- `Column`  
The column to Aggregate or Order By in the form _table.alias_.

## Options
- `-rm|--remove`  
Remove column.
- `-a|--aggregation`  
  Must be one of:  
  `SUM`  
  `MIN`  
  `MAX`  
  `AVG`  
  `COUNT`  
  `COUNT_DISTINCT`  
  `AVG_DISTINCT`
- `-o|--order`  
  Either `ASC` or `DESC`.

#### See [Oracle's Reference](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC233) for more.

