# Stack Action Usage
### [Command](./cmd-sfm.md) `sa`
```
Usage: sfm sa [options] [command]

Options:
  -rc|--returnControlIds  Return Control IDs
  -fo|--formOID           Open Form Id
  -sw|--stopOnWarning     Stop on Warning
  -?|-h|--help            Show help information

Commands:
  fa                      Stack Form Action
```
## Commands
- [`fa` - Stack Form Action](./cmd-fa.md)

## Options
See [Oracle's documentation for reference.](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC132)

## Examples

### Create a new Stack Action _test_
```
[:] $ sfm -c test sa -fo w01012b -rc 28
New Stack Form Definition? [y/N] y
[:] sfm:test $ 
```
