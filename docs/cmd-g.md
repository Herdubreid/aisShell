# Grid Insert/Update Usage
### [Command](./cmd-fm.md) `gi` and `gu`
```
Usage: [fm|sfm fr] [gi|gu] [arguments] [options]

Arguments:
  ColumnID        Control Id
  Command         Command
  Value           Value

Options:
  -g|--gridId     Grid Id
  -r|--rowNumber  Row Number
  -rm|--remove    Remove Column Event
  -?|-h|--help    Show help information
```

## Arguments
- `ColumnID`  
  Grid Column ID receiving the command.
- `Command`  
  Must be either `SetGridCellValue` or `SetGridComboValue`.
- `Value`  
  The value of the command.

## Options
- `g|--gridId`  
  The Grid ID, typically _1_ for single grid Forms.
- `r|--rowNumber`  
  The grid row number of the command.
- `-rm|--rm`  
  Remove a `ColumnID` event.

## Examples

### Add new Grid Row in Empty Grid
```
$ gi -g 1 -r 1
$ gi 27 SetGridCellValue HOM          
$ gi 28 SetGridCellValue 303               
$ gi 29 SetGridCellValue 123-456
```
The first line adds `gridRowInsertEvents` to Grid Id _1_ with `gridColumnEvents` of Row _1_, to which the next 3 lines are subsequently added (the _-g 1 -r 1_ options are reduntant).

### Update an Existing Grid Row
```
$ gu -g 1 -r 1
$ gi 27 SetGridCellValue HOM          
$ gi 28 SetGridCellValue 303               
$ gi 29 SetGridCellValue 123-456
```
This is identical to the above example, except with `gridRowUpdateEvents` added to Grid Id _1_.

### Remove Grid Column Event
```
$ gi -g 1 -r 1 -rm 28
```
Removes `ColumnID` _28_ from Row _1_ `gridRowInsertEvents` in Grid Id _1_ (replace `gi` with `gu` for `gridRowUpdateEvents`, ).

#### See [Oracle's Reference](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC180) for more.