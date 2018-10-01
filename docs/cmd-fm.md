# Form Context Usage
### [Command](./cmds.md) `fm`
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

## Options

- [`-c|--context and -|--listContexts`](./opt-context-and-list.md)

## Commands

- [`d` - Form Definition](./cmd-fm-d.md)
- [`fa` - Form Action](./cmd-fa.md)
- [`fi` - Form Input](./cmd-fi.md)
- [`gi` - Grid Insert](./cmd-g.md)
- [`gu` - Grid Update](./cmd-g.md)
- [`qry` - Query](./cmd-qry.md)
- [`exp` - Export Request](./cmd-exp.md)
- [`r` - Response](./cmd-r.md)
- [`save` and `load`](./cmd-save-and-load.md)
- `s` - Submit  
  Submits the Form Request.  If successful, the response can be explored with the `r` command.

## Examples

### Create a new `Form Context` _wwab_: 
```
[e1:demo] $ fm -c wwab d -fn p01012_w01012b -mp 30 -rc 54|1[19,20]
New Form Definition? [Y/n] y
[e1:demo] fm:wwab $ 
```
This is now the the default context, showing in the prompt with _fm:wwab_.

### Add Form Actions
```
[e1:demo] fm:wwab $ fa 54 SetControlValue E
[e1:demo] fm:wwab $ fa 15 DoAction
[e1:demo] fm:wwab $ 
```
The `Search Type` is set to 'E' and press the find button pressed.

### Submit Form Request
```
[e1:demo] fm:wwab $ s
.Responses 1.
[e1:demo] fm:wwab $ 
```
Because the `Form Context` is default the `fm` command can be omitted.  The response can be explored with the [`r` command](./cmd_r.md).
