# Response [Usage](../README.md#commands)
### Subcommand `r`
```
Usage: [fm|d|sfm] r [options] [command]

Options:
  -i|--index     Zero Based Index
  -k|--key       Object Key
  -d|--depth     Iteration Depth
  -of|--outFile  Write Result to File
  -?|-h|--help   Show help information

Commands:
  it             Iterate
```
## Commands
- [`it` - Iterates](./cmd-it.md)

## Options
- `-i|--index`  
The context keeps a indexed repository of responses, so earlier responses can be explored.  If omitted the last reponse defaults.
- `-k|--key`  
  Export a Json key value.
- `-d|--depth`  
  Limit export to to _depth_ levels.
- `-of|--outFile`  
  Redirect eport to _file_.  This overrides the `out` command's default file.

## Examples

### Explore last Response's Top Level
```
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
```
the `-d 0` option displays only the first response level (0 depth).

### Explore last Response Json Key
```
[e1:demo] fm:wwab $ r -k summary
{
  "records": 0,
  "moreRecords": false
}
[e1:demo] fm:wwab $ 
```
The `-k summary` option extracts the `summary` key of the response.
