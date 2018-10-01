# Deleting Data [Example](../README.md#examples)

#### See [`Server Context`](./cmd-sv.md#examples) for _e1_ connection example.

#### See [Oracle Doc's](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC182) for Java example.

### Create Form Request
```
[e1:demo] $ fm -c delPhone d -fn p0115_w0115a -fs u
New Form Definition? [y/N] y
[e1:demo] fm:delPhone $ 
```

### Add Form Input Values
```
[e1:demo] fm:delPhone $ fi 4 7500                 
[e1:demo] fm:delPhone $ fi 5 0
[e1:demo] fm:delPhone $ 
```
The values _7500_ respresents the Address Number and _0_ the Who's Who line number.

### Add Form Actions and Submit
```
[e1:demo] fm:delPhone $ fa 1.0 SelectRow
[e1:demo] fm:delPhone $ fa 59 DoAction
[e1:demo] fm:delPhone $ fa 4 DoAction
[e1:demo] fm:delPhone $ s
/Responses 1.
[e1:demo] fm:delPhone $ 
```
The actions are: Select Row _0_, press the Delete button (control _59_) and finally press the Ok button (control _4_).

### Test if Grid is Empty
```
[e1:demo] fm:delPhone $ r -k summary
{
  "records": 0,
  "moreRecords": false
}
[e1:demo] fm:delPhone $ 
```
