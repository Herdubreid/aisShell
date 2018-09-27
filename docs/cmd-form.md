# Usage
### Command `fm`.
```
[:] $ fm -h
Form Context

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

Run 'fm [command] --help' for more information about a command.
```
An AIS Form Request definition.
## Usage
### Options
- [`-c|--context and -|--listContexts`](./opt-context-and-list.md)
### Commands
- [`d` - Form Definition](./opt-fr.md)
- [`exp` - Export](./cmd-exp.md)
- [`fa` - Form Action](./cmd-fa.md)
- [`fi` - Form Input](./cmd-fi.md)
- [`gi` - Grid Insert](./cmd-gi.md)
- [`gu` - Grid Update](./cmd-gu.md)
- [`save` and `load`](./cmd-save-and-load.md)
- [`r` Response](./cmd-respones.md)
- [`s` Submit](./cmd-submit.md)  

Example:
```csh
[e1:] $ fm -c wwab d -fn p01012_w01012b -mp 30 -rc 54|1[19,20]       
New Form Definition? [Y/n] y
[e1:] fm:wwab $ 
```
Where:
- `-c` and `-d` for context and definition as above.
- `-fn` is form name.
- `-mp` is max returned pages (rows).
- `-rc` is a list of control Ids to return.

Before a form can be submitted, the server context must be connected:
```csh
[e1:] fm:wwab $ sv c
User name: demo
Password: *******
..
Signon success!
[e1:demo] fm:wwab $ 
```

A connected (or authenticated) server context displayes the user name in the prompt `[e1:demo]`.  The current context `fm:wwab` is the form context that was just created.

The form is submitted with the `fm s` command (the `fm` is redundant for the current context).
```csh
[e1:demo] fm:wwab $ s
.Responses 1.
[e1:demo] fm:wwab $ 
```
The response can be explored with the `r` command.
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
