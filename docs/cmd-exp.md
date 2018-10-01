# Export Request [Usage](../README.md#commands)
# Subcommand `exp`
```
Usage: [sv|fm|sfm|dt] exp [options] [command]

Options:
  -k|--key       Object Key
  -d|--depth     Iteration Depth
  -of|--outFile  Write Result to File
  -?|-h|--help   Show help information

Commands:
  it             Iterate
```
## Options
- `-k|--key`  
  Export a Json key value.
- `-d|--depth`  
  Limit export to to _depth_ levels.
- `-of|--outFile`  
  Redirect eport to _file_.  This overrides the `out` command's default file.

## Commands
- [`it` - Iterates](./cmd-it.md)

## Examples

### Display Request
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
```

### Display Request's Form Actions
```
[e1:demo] fm:wwab $ exp -k formActions       
{
  "controlID": "54",
  "command": "SetControlValue",
  "value": "E"
}
{
  "controlID": "15",
  "command": "DoAction",
  "value": ""
}
```