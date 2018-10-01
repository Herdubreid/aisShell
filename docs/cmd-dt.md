# Data Context [Usage](../README.md)
### Command `dt`
```
Usage: dt [options] [command]

Options:
  -c|--context       Context Id
  -l|--listContexts  List Contexts
  -?|-h|--help       Show help information

Commands:
  d                  Define
  exp                Export Request
  load               Load Definition
  qry                Query
  r                  Response
  s                  Submit Request
  save               Save Definition
```
An AIS Data Request definition.

**Note:** The `Server Context` requires the `dataservice` capability.

## Options
- [`-c|--context and -|--listContexts`](./opt-context-and-list.md)

## Commands
- [`dt` Define](./cmd-dt-d.md)
- [`qry` Query](./cmd-qry.md)
- [`exp` Export Request](./cmd-exp.md)
- [`r` Response](./cmd-r.md)
- [`save` and `load`](./cmd-save-and-load.md)
- `s` - Submit  
Submits the Data Request. If successful, the response can be explored with the `r` command.

## Examples

### Create view request for V0101
```
[:] dt:ab $ dt -c abvw d v01010 view           
New Data Definition? [y/N] y
[:] dt:abvw $
```
