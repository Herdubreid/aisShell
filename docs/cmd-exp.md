# Export Request Usage
# [Command](./cmds.md) `exp`
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

### 