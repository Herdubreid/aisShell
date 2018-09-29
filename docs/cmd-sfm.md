# Stack Form Context Usage
### [Command](./cmds.md) `sfm`
```
Usage: sfm [options] [command]

Options:
  -c|--context       Context Id
  -l|--listContexts  List Contexts
  -?|-h|--help       Show help information

Commands:
  c                  Close Form
  e                  Execute Stack Action
  exp                Export Request
  fr                 Form Request
  load               Load Definition
  o                  Open Form
  r                  Response
  sa                 Stack Action
  save               Save Definition
```
An AIS Stack Application Request.
### Options
- [`-c|--context and -|--listContexts`](./opt-context-and-list.md)
### Commands
- `o` - Opens the form request.
- `e` - Executes the stack action.
- `c` - Closes an active stack request.
- [`fr` - Form Request](./cmd-fm-d.md)
- [`sa` - Stack Action](./cmd-sa.md)
- [`exp` - Export](./cmd-exp.md)
- [`save` and `load`](./cmd-save-and-load.md)
