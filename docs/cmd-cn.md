# Condition [Usage](../README.md#commands)
### [Subcommand](./cmd-qry.md) `cn`
```
Usage: [fm|sfm|dt] [qry|cpq] cn [arguments] [options]

Arguments:
  ControlId     Control Id
  Operator      Operator
  ValueType     Value Type
  Value         Value  (separate multple values with ';')

Options:
  -rm|--remove  Remove Condition
  -?|-h|--help  Show help information
```
## Arguments
- `ControlId`  
The Control Id to apply the condition to.
- `Operator`  
  Must be one of:  
  `BETWEEN`  
  `LIST`  
  `EQUAL`  
  `NOT_EQUAL`  
  `LESS`  
  `LESS_EQUAL`  
  `GREATER`  
  `GREATER_EQUAL`  
  And for strings only:  
  `STR_START_WITH`  
  `STR_END_WITH`  
  `STR_CONTAIN`  
  `STR_BLANK`  
  `STR_NOT_BLANK`
- `ValueType`  
  Either `LITERAL` for value comparison or one of:  
  `TODAY`  
  `TODAY_PLUS_DAY`  
  `TODAY_MINUS_DAY`  
  `TODAY_PLUS_MONTH`  
  `TODAY_MINUS_MONTH`  
  `TODAY_PLUS_YEAR`  
  `TODAY_MINUS_YEAR`

## Options
- `-rm|--remove`  
   Remove a `ControlId` condition.

## Examples

### Add Query with Between Conditon
```
[:] dt:abw $ qry -mt match_all -af true
[:] dt:abw $ qry cn F0101.AN8 between literal 6000;6500
```
The first line creates a `MATCH_ALL` query with `autoFind` _true_ and the second line add a condition where AN8 alias is between _6000_ and _6500_.
