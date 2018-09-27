# Form Context Usage
### Command `fm`
```
Usage: fm [options] [command]

Options:
  -c|--context       Context Id
  -l|--listContexts  List Contexts
  -?|-h|--help       Show help information

Commands:
  d                  Form Definition
  exp                Export Request
  fa                 Form Action
  fi                 Form Input
  gi                 Grid Insert
  gu                 Grid Update
  load               Load Definitions
  qry                Query
  r                  Response
  s                  Submit Request
  save               Save Definitions
```
An AIS Form Request definition.
### Options
- [`-c|--context and -|--listContexts`](./opt-context-and-list.md)
### Commands
- [`d` - Form Definition](./cmd-fm-d.md)
- [`fa` - Form Action](./cmd-fa.md)
- [`fi` - Form Input](./cmd-fi.md)
- [`gi` - Grid Insert](./cmd-gi.md)
- [`gu` - Grid Update](./cmd-gu.md)
- [`save` and `load`](./cmd-save-and-load.md)
- [`exp` - Export Request](./cmd-exp.md)
- [`s` Submit](./cmd-submit.md)  
- [`r` Response](./cmd-respones.md)

### Examples
#### Create a new `Form Context` _wwab_: 
```
[e1:demo] $ fm -c wwab d -fn p01012_w01012b -mp 30 -rc 54|1[19,20]
New Form Definition? [Y/n] y
[e1:demo] fm:wwab $ 
```
This is now the the default context, showing in the prompt with _fm:wwab_.
#### Submit Form Request
```csh
[e1:demo] fm:wwab $ s
.Responses 1.
[e1:demo] fm:wwab $ 
```
Because the `Form Context` is default the `fm` command can be omitted.
#### Explore the Response
```csh
[e1:demo] fm:wwab $ r -d 0
{
  "fs_P01012_W01012B": {},
  "stackId": 1,
  "stateId": 1,
  "rid": "fdcff8fa532d1fd6",
  "currentApp": "P01012_W01012B",
  "timeStamp": "2018-09-26:17.31.06",
  "sysErrors": []
}
[e1:demo] fm:wwab $ r -k summary
{
  "records": 0,
  "moreRecords": false
}
[e1:demo] fm:wwab $ 
```
the `r -d 0` command displays only the first response level (0 depth).  The `r -k summary` command displays the `summary` of the response, which has 0 grid rows.

To return some records we need set the `Search Type` to 'E' and press the find button.
```csh
[e1:demo] fm:wwab $ fa 54 SetControlValue E
[e1:demo] fm:wwab $ fa 15 DoAction
[e1:demo] fm:wwab $ s
.....Responses 2.
[e1:demo] fm:wwab $ r -k summary
{
  "records": 30,
  "moreRecords": true
}
[e1:demo] fm:wwab $ 
```
The `fa` command accepts `ControlID`, `Command` and optionally `Value`.  The response now has 30 records.
```csh
[e1:demo] fm:wwab $ r -k rowset[0] -d 0                  
{
  "rowIndex": 0,
  "MOExist": false,
  "mnAddressNumber_19": {},
  "sAlphaName_20": {}
}
[e1:demo] fm:wwab $
```
The two requested grid controls `1[19,20]` are `mnAddressNumber_19` and `sAlphaName_20`.  We can explore their value with the `it` command.
```csh
[e1:demo] fm:wwab $ r -k rowset it mnAddressNumber_19.value;sAlphaName_20.value
1001    AB Common   
1234    Long, Ben   
2006...
...     Abbott, Dominique   
6016    Hunter, Monica  
[e1:demo] fm:wwab $ 
```
