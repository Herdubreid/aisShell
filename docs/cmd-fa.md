# Form Action [Usage](../README.md#commands)
### Subcommand `fa`
```
Usage: [fm|fr] fa [arguments] [options]

Arguments:
  ControlID      Control Id
  Command        Command
  Value          Value

Options:
  -i|--index     Add or Remove Zero-based Index
  -rm|--remove   Remove Form Action
  -?|-h|--help   Show help information
```

## Arguments
- `ControlID`  
  The AIS Control ID receiving the command.
- `Command`  
  Must be one of:  
  `SetControlValue`  
  `SetQBEValue`  
  `DoAction`  
  `SetRadioButton`  
  `SetComboValue`  
  `SetCheckboxValue`  
  `SelectRow`  
  `UnSelectRow`  
  `UnSelectAllRows`  
  `SelectAllRows`  
  `ClickGridCell`  
  `ClickGridColumnAggregate`  
- `Value`  
  Optional value with the command.

## Options
- `-i|--index`  
  A zero based index to either remove action or add.
- `-rm|--remove`  
  Remove action with `ControlID` or index.

## Examples

### Add a Press Find Action
```
fa 15 DoAction
```
Assuming `ControlId` _15_ represents the find button.

### Remove the First Action
```
$ fa -i 0 -rm
```

### Insert Setting a Form Value as the First Action
```
$ fa -i 0 54 SetControlValue E
```

#### See [Oracle's Reference](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC128) for more.
