# Iterate [Usage](../README.md)
### [Command](./cmd-exp.md) `it`
```
Usage: [exp|r] it [arguments] [options]

Arguments:
  Key            Key (separate multiple keys with ';'
  From           From
  To             To

Options:
  -of|--outFile  Write Result to File
  -?|-h|--help   Show help information
```

## Arguments
- `Key`  
A semicolon seperated list of Json key's to export from the array.
- `From`  
Iterate from zero based row.
- `To`  
Iterate to zero based row.

## Options
- `-of|--outFile`  
  Redirect eport to _file_.  This overrides the `out` command's default file.

## Examples

### Explor Form Request
```
[e1:demo] fm:wwab $ exp
{
  "formServiceAction": "R",
  "formInputs": [],
  "formActions": [
    {
      "controlID": "54",
      "command": "SetControlValue",
      "value": "E"
    },
    {
      "controlID": "15",
      "command": "DoAction",
      "value": ""
    }
  ],
  "formName": "P01012_W01012B",
  "returnControlIDs": "54|1[19,29]",
  "maxPageSize": "30",
  "aliasNaming": false
}
[:] fm:wwab $
```

### Show the First Grid Row of Response
```
[e1:demo] fm:wwab $ r -k rowset[0] -d 0                  
{
  "rowIndex": 0,
  "MOExist": false,
  "mnAddressNumber_19": {},
  "sAlphaName_20": {}
}
[e1:demo] fm:wwab $
```

### Show Grid Row Values
```
[e1:demo] fm:wwab $ r -k rowset it mnAddressNumber_19.value;sAlphaName_20.value
1001    AB Common   
1234    Long, Ben   
2006...
...     Abbott, Dominique   
6016    Hunter, Monica  
[e1:demo] fm:wwab $ 
```
