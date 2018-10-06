# Data Context [Usage](../README.md#commands)
### Command `dt`
```
Usage: dt [options] [command]

Options:
  -c|--context       Context Id
  -l|--listContexts  List Contexts
  -?|-h|--help       Show help information

Commands:
  ag                 Aggregate
  cpq                Complex Query
  d                  Define
  exp                Export Request
  gr                 Group By
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
- [`d` Define](./cmd-dt-d.md)
- [`qry` Query](./cmd-qry.md)
- [`cpq` Complex Query](./cmd-cpq.md)
- [`ag` Aggregate](./cmd-ag.md)
- [`gr` Group](./cmd-gr.md)
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
