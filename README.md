# aisShell
A command line Utility for AIS

## Commands
```bash
$ help
Usage: [command] [options]

Commands:
  sv                   Server Context
  fm                   Form Context
  sfm                  Stack Form Context
  dt                   Data Context
  out                  Set Output File
  help                 Show Available Commands
  clear                Clear Screen
  quit                 Quit Shell
```
### Server Context `sv`
An AIS server definition.
Example:
```csh
[:] $ sv -c e1 d -b http://e1.celin.io:9300/jderest/
Server Context 'e1' not found!
New Server Definition? [Y/n] y
[e1:] $ 
```
Where:
- `-c` is Context Id `e1`.  If it doesn't exist then the user is prompted for a new definition.
- `d` Context definition.
- `-b` is the the AIS Url (note trailing '/').

Command usage help is displayed with trailing `-?` or `-h`.  For example `sv -h` and `sv d -?`.

The current server context is displayed in the prompt `[e1:] $`.

### Form Context `fm`.
An AIS Form Request definition.
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
The two grid requested grid controls `1[19,20]` are `mnAddressNumber_19` and `sAlphaName_20`.  We can explore their value with the `it` command.
```csh
[e1:demo] fm:wwab $ r -k rowset it mnAddressNumber_19.value;sAlphaName_20.value
1001	AB Common	
1234	Long, Ben	
2006...
...	    Abbott, Dominique	
6016	Hunter, Monica	
[e1:demo] fm:wwab $ 
```csh
