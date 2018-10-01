# Condition [Usage](../README.md)
### [Command](./cmd-qry.md) `qry cn`
```
Usage: [fm|sfm|dt] qry cn [arguments] [options]

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

  ### Add Between Conditon
  ```
  ```